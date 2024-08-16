namespace CalibrationLab.Controllers
{

    using CalibrationLab.Models;
    using Microsoft.AspNetCore.Mvc;
    using MySql.Data.MySqlClient;
    using System.Data.Common;

    [ApiController]
    [Route("api/[controller]")]
    public class UserAPIController(ILogger<UserAPIController> logger, IConfiguration configuration) : ControllerBase
    {

        private readonly ILogger<UserAPIController> _logger = logger;
        private readonly string? _connectionString = configuration.GetConnectionString("DefaultConnection");


        [HttpPost("addUser")]
        public async Task<IActionResult> AddUser([FromForm] User user)
        {
            using (MySqlConnection connection = new(_connectionString))
            {
                await connection.OpenAsync();
                using (MySqlCommand checkUserCommand = new())
                {
                    checkUserCommand.Connection = connection;
                    checkUserCommand.CommandText = @"
                        SELECT COUNT(*) FROM users
                        WHERE mail = @Mail OR employee_id = @EmployeeId
                    ";
                    checkUserCommand.Parameters.AddWithValue("@Mail", user.Mail);
                    checkUserCommand.Parameters.AddWithValue("@EmployeeId", user.EmployeeId);
                    int existingUserCount = Convert.ToInt32(await checkUserCommand.ExecuteScalarAsync());
                    if (existingUserCount > 0)
                    {
                        return Conflict(new { Message = "User with this email or employee ID already exists." });
                    }
                }

                if (user.Signature != null)
                {
                    using MemoryStream memoryStream = new();
                    await user.Signature.CopyToAsync(memoryStream);
                    user.Sign = memoryStream.ToArray();
                }

                using (MySqlCommand command = new())
                {
                    command.Connection = connection;
                    command.CommandText = @"
                        INSERT INTO users (
                            name,
                            employee_id,
                            mail,
                            password,
                            signature
                        )
                        VALUES (@Name, @EmployeeId, @Mail, @Password, @Signature)
                    ";
                    command.Parameters.AddWithValue("@Name", user.Name);
                    command.Parameters.AddWithValue("@EmployeeId", user.EmployeeId);
                    command.Parameters.AddWithValue("@Mail", user.Mail);
                    command.Parameters.AddWithValue("@Password", BCrypt.Net.BCrypt.HashPassword(user.Password, 12));
                    command.Parameters.AddWithValue("@Signature", user.Sign);
                    await command.ExecuteNonQueryAsync();
                }
            }
            return Ok();
        }


        [HttpGet("getUser")]
        public async Task<IActionResult> GetUser([FromQuery] string employeeId)
        {
            if (string.IsNullOrEmpty(employeeId))
                return Ok(null);
            User? user = await GetUserFromDb(employeeId);
            if(user == null)
            {
                return NotFound();
            }
            string? base64String = user.Sign != null ? Convert.ToBase64String(user.Sign) : null;
            return Ok(new
            {
                user.Name,
                user.EmployeeId,
                user.Mail,
                Base64Image = base64String
            });
        }

        //[HttpPost]
        //public async Task<IActionResult> UpdateUser(User user)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        byte[]? signByte = null;
        //        if (user.Signature != null)
        //        {
        //            using MemoryStream memoryStream = new();
        //            await user.Signature.CopyToAsync(memoryStream);
        //            signByte = memoryStream.ToArray();
        //        }
        //        user.Password = new PasswordHelper().HashPassword(user.Password);
        //        UpdateUserDb(user);
        //        return RedirectToAction("Index"); // Redirect to a different view after successful update
        //    }
        //    return Ok();
        //}



        //private void UpdateUserDb(User user)
        //{
        //    using MySqlConnection connection = new(_connectionString);
        //    connection.Open();
        //    using MySqlCommand command = new();
        //    command.Connection = connection;
        //    command.CommandText = @"
        //            UPDATE users
        //            SET name = @Name,
        //                mail = @Mail,
        //                password = @Password,
        //                signature = @Signature
        //            WHERE employee_id = @EmployeeId";

        //    command.Parameters.AddWithValue("@Name", user.Name);
        //    command.Parameters.AddWithValue("@Mail", user.Mail);
        //    command.Parameters.AddWithValue("@Password", user.Password);
        //    command.Parameters.AddWithValue("@Signature", user.Signature);
        //    command.Parameters.AddWithValue("@EmployeeId", user.EmployeeId);
        //    command.ExecuteNonQuery();
        //}


        private async Task<User?> GetUserFromDb(string employeeId)
        {
            using MySqlConnection connection = new(_connectionString);
            await connection.OpenAsync();

            using MySqlCommand command = new("SELECT * FROM users WHERE employee_id = @EmployeeId", connection);
            command.Parameters.AddWithValue("@EmployeeId", employeeId);

            using DbDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Name = reader["name"].ToString()!,
                    EmployeeId = reader["employee_id"].ToString()!,
                    Mail = reader["mail"].ToString()!,
                    Password = "Password",
                    Sign = (byte[])reader["signature"]
                };
            }
            return null;
        }
    }
}
namespace CalibrationLab.Controllers
{

    using CalibrationLab.Models;
    using Microsoft.AspNetCore.Mvc;
    using MySql.Data.MySqlClient;


    [ApiController]
    [Route("api/[controller]")]
    public class UserAPIController(ILogger<UserAPIController> logger) : ControllerBase
    {

        private readonly ILogger<UserAPIController> _logger = logger;
        private readonly string _connectionString = Constants.ConnectionString;


        [HttpPost("addUser")]
        public async Task<IActionResult> AddUser([FromForm] User user)
        {

            if (user.Signature != null)
            {
                using MemoryStream memoryStream = new();
                await user.Signature.CopyToAsync(memoryStream);
                user.Sign = memoryStream.ToArray();
            }
            using (MySqlConnection connection = new(_connectionString))
            {
                connection.Open();
                using MySqlCommand command = new();
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
                command.ExecuteNonQuery();
            }
            return Ok();
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


        //[HttpGet]
        //public async Task<IActionResult> GetUser(string employeeId)
        //{
        //    if (string.IsNullOrEmpty(employeeId))
        //        return Ok(null);
        //    User user = await GetUserFromDb(employeeId);
        //    return Ok(user);
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


        //private async Task<User> GetUserFromDb(string employeeId)
        //{
        //    using MySqlConnection connection = new(_connectionString);
        //    await connection.OpenAsync();

        //    using MySqlCommand command = new("SELECT * FROM users WHERE EmployeeId = @EmployeeId", connection);
        //    command.Parameters.AddWithValue("@EmployeeId", employeeId);

        //    using DbDataReader reader = await command.ExecuteReaderAsync();
        //    if (await reader.ReadAsync())
        //    {
        //        return new User
        //        {
        //            Name = reader["Name"].ToString()!,
        //            EmployeeId = reader["EmployeeId"].ToString()!,
        //            Mail = reader["Mail"].ToString()!,
        //            Password = reader["Password"].ToString()!,
        //            Sign = (byte[])reader["Signature"]
        //        };
        //    }
        //    return null;
        //}
    }
}
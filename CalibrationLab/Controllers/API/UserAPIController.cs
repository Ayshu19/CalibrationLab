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
                user.Id,
                user.Name,
                user.EmployeeId,
                user.Mail,
                Base64Image = base64String
            });
        }


        [HttpPost("updateUser")]
        public async Task<IActionResult> UpdateUser([FromForm] User user)
        {
            using (MySqlConnection connection = new(_connectionString))
            {
                await connection.OpenAsync();
                using (MySqlCommand checkUserCommand = new())
                {
                    checkUserCommand.Connection = connection;
                    checkUserCommand.CommandText = @"
                        SELECT COUNT(*)
                        FROM users
                        WHERE (mail = @Mail OR employee_id = @EmployeeId)
                        AND id != @UserId
                    ";
                    checkUserCommand.Parameters.AddWithValue("@Mail", user.Mail);
                    checkUserCommand.Parameters.AddWithValue("@EmployeeId", user.EmployeeId);
                    checkUserCommand.Parameters.AddWithValue("@UserId", user.Id);
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
                    using (MySqlCommand command = new())
                    {
                        command.Connection = connection;
                        command.CommandText = @"
                        UPDATE users
                        SET 
                            signature = @Signature
                        WHERE 
                            id = @UserId
                        ";
                        command.Parameters.AddWithValue("@UserId", user.Id);
                        command.Parameters.AddWithValue("@Signature", user.Sign);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                if (user.Password != null) {
                    using (MySqlCommand command = new())
                    {
                        command.Connection = connection;
                        command.CommandText = @"
                        UPDATE users
                        SET 
                            password = @Password
                        WHERE 
                            id = @UserId
                    ";
                        command.Parameters.AddWithValue("@UserId", user.Id);
                        command.Parameters.AddWithValue("@Password", BCrypt.Net.BCrypt.HashPassword(user.Password, 12));
                        await command.ExecuteNonQueryAsync();
                    }
                }

                using (MySqlCommand command = new())
                {
                    command.Connection = connection;
                    command.CommandText = @"
                        UPDATE users
                        SET 
                            name = @Name,
                            mail = @Mail,
                            employee_id = @EmployeeId
                        WHERE 
                            id = @UserId
                    ";
                    command.Parameters.AddWithValue("@UserId", user.Id);
                    command.Parameters.AddWithValue("@Name", user.Name);
                    command.Parameters.AddWithValue("@EmployeeId", user.EmployeeId);
                    command.Parameters.AddWithValue("@Mail", user.Mail);
                    await command.ExecuteNonQueryAsync();
                }
            }
            return Ok();
        }


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
                    Id = (int)reader["id"],
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
namespace CalibrationLab.Controllers
{

    using CalibrationLab.Models;
    using CalibrationLab.Utilities;
    using Microsoft.AspNetCore.Mvc;
    using MySql.Data.MySqlClient;
    using System.Data.Common;


    public class UserController(ILogger<UserController> logger) : Controller
    {

        private readonly ILogger<UserController> _logger = logger;
        private readonly string _connectionString = Constants.ConnectionString;
        private readonly PasswordHelper _passwordHelper = new();


        public IActionResult Add()
        {
            return View();
        }


        public IActionResult Edit()
        {
            return View();
        }


        public IActionResult Delete()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddUser(User user, IFormFile Signature)
        {
            if (ModelState.IsValid)
            {
                if (Signature != null)
                {
                    using MemoryStream memoryStream = new();
                    await Signature.CopyToAsync(memoryStream);
                    user.Signature = memoryStream.ToArray();
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
                    command.Parameters.AddWithValue("@Password", _passwordHelper.HashPassword(user.Password));
                    command.Parameters.AddWithValue("@Signature", user.Signature);
                    command.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            return View(user);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateUser(User model, IFormFile Signature)
        {
            if (ModelState.IsValid)
            {
                if (Signature != null)
                {
                    using MemoryStream memoryStream = new();
                    await Signature.CopyToAsync(memoryStream);
                    model.Signature = memoryStream.ToArray();
                }
                model.Password = new PasswordHelper().HashPassword(model.Password);
                UpdateUser(model);
                return RedirectToAction("Index"); // Redirect to a different view after successful update
            }
            return View("EditUser", model);
        }


        [HttpGet]
        public async Task<IActionResult> GetUser(string employeeId)
        {
            if (string.IsNullOrEmpty(employeeId))
                return Json(null);
            User user = await GetUserFromDb(employeeId);
            return Json(user);
        }


        private void UpdateUser(User user)
        {
            using MySqlConnection connection = new(_connectionString);
            connection.Open();
            using MySqlCommand command = new();
            command.Connection = connection;
            command.CommandText = @"
                    UPDATE users
                    SET name = @Name,
                        mail = @Mail,
                        password = @Password,
                        signature = @Signature
                    WHERE employee_id = @EmployeeId";

            command.Parameters.AddWithValue("@Name", user.Name);
            command.Parameters.AddWithValue("@Mail", user.Mail);
            command.Parameters.AddWithValue("@Password", user.Password);
            command.Parameters.AddWithValue("@Signature", user.Signature);
            command.Parameters.AddWithValue("@EmployeeId", user.EmployeeId);
            command.ExecuteNonQuery();
        }


        private async Task<User> GetUserFromDb(string employeeId)
        {
            using MySqlConnection connection = new(_connectionString);
            await connection.OpenAsync();

            using MySqlCommand command = new("SELECT * FROM users WHERE EmployeeId = @EmployeeId", connection);
            command.Parameters.AddWithValue("@EmployeeId", employeeId);

            using DbDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Name = reader["Name"].ToString()!,
                    EmployeeId = reader["EmployeeId"].ToString()!,
                    Mail = reader["Mail"].ToString()!,
                    Password = reader["Password"].ToString()!,
                    Signature = (byte[])reader["Signature"]
                };
            }
            return null;
        }
    }
}
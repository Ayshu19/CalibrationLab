namespace CalibrationLab.Controllers
{
    using CalibrationLab.Models;
    using Microsoft.AspNetCore.Mvc;
    using MySql.Data.MySqlClient;

    public class UserController(ILogger<UserController> logger) : Controller
    {

        private readonly ILogger<UserController> _logger = logger;
        private string _connectionString = "server=your_server;user=your_user;password=your_password;database=your_database;";


        public IActionResult Add()
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
                    using var memoryStream = new MemoryStream();
                    await Signature.CopyToAsync(memoryStream);
                    user.Signature = memoryStream.ToArray();
                }

                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new MySqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = @"
                    INSERT INTO users (Name, EmployeeId, Mail, Password, Signature)
                    VALUES (@Name, @EmployeeId, @Mail, @Password, @Signature)";

                        command.Parameters.AddWithValue("@Name", user.Name);
                        command.Parameters.AddWithValue("@EmployeeId", user.EmployeeId);
                        command.Parameters.AddWithValue("@Mail", user.Mail);
                        command.Parameters.AddWithValue("@Password", user.Password);
                        command.Parameters.AddWithValue("@Signature", user.Signature);

                        command.ExecuteNonQuery();
                    }
                }
                return RedirectToAction("Index");
            }
            return View(user);
        }

        public IActionResult Edit()
        {
            return View();
        }


        public IActionResult Delete()
        {
            return View();
        }
    }
}
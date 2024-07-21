namespace CalibrationLab.Controllers
{

    using CalibrationLab.Models;
    using CalibrationLab.Utilities;
    using Microsoft.AspNetCore.Mvc;


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
    }
}
namespace CalibrationLab.Controllers
{

    using Microsoft.AspNetCore.Mvc;


    public class UserController(ILogger<UserController> logger) : Controller
    {

        private readonly ILogger<UserController> _logger = logger;


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
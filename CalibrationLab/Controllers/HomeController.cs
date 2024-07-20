namespace CalibrationLab.Controllers
{

    using Microsoft.AspNetCore.Mvc;


    public class HomeController(ILogger<HomeController> logger) : Controller
    {

        private readonly ILogger<HomeController> _logger = logger;


        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }
    }
}
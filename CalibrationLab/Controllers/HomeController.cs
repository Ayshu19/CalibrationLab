namespace CalibrationLab.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]

    public class HomeController(ILogger<HomeController> logger) : Controller
    {

        private readonly ILogger<HomeController> _logger = logger;


        public IActionResult Index()
        {
            return View();
        }
    }
}
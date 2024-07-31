namespace CalibrationLab.Controllers
{
    using Microsoft.AspNetCore.Mvc;


    public class AccountController(ILogger<AccountController> logger) : Controller
    {

        private readonly ILogger<AccountController> _logger = logger;


        public IActionResult Login()
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}
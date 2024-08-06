namespace CalibrationLab.Controllers
{
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication;
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


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
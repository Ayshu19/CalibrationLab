namespace CalibrationLab.Controllers
{
    using CalibrationLab.Models;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc;
    using MySql.Data.MySqlClient;
    using System.Data;
    using System.Security.Claims;


    [ApiController]
    [Route("api/[controller]")]
    public class AccountAPIController(IConfiguration configuration) : ControllerBase
    {
        private readonly string? _connectionString = configuration.GetConnectionString("DefaultConnection");

        [HttpPost("signIn")]
        public async Task<IActionResult> SignIn([FromForm] SignInRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { Message = "Invalid UserName / Password" });
            }
            if (request.UserName == "admin" && request.Password == "Admin@123") //REMOVE
            {
                List<Claim> claims = [
                        new Claim(ClaimTypes.Name, request.UserName),
                        new Claim(ClaimTypes.NameIdentifier, request.UserName)
                    ];
                ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                AuthenticationProperties authProperties = new()
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                    IsPersistent = true,
                    IssuedUtc = DateTimeOffset.UtcNow,
                    RedirectUri = "/"
                };
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return Ok(new { IsSuccess = true, Message = "Login Successful!" });
            }

            using MySqlConnection connection = new(_connectionString);
            await connection.OpenAsync();

            using MySqlCommand command = new("SELECT * FROM users WHERE mail = @mail", connection);
            command.Parameters.AddWithValue("@mail", request.UserName);

            using MySqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);
            if (await reader.ReadAsync())
            {
                User user = new()
                {
                    Name = reader.GetString("name"),
                    EmployeeId = reader.GetString("employee_id"),
                    Mail = reader.GetString("mail"),
                    Password = reader.GetString("password"),
                };

                if (BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                {
                    List<Claim> claims = [
                        new Claim(ClaimTypes.Name, user.Mail),
                        new Claim(ClaimTypes.NameIdentifier, user.EmployeeId)
                    ];
                    ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    AuthenticationProperties authProperties = new()
                    {
                        AllowRefresh = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                        IsPersistent = true,
                        IssuedUtc = DateTimeOffset.UtcNow,
                        RedirectUri = "/"
                    };
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return Ok(new { IsSuccess = true, Message = "Login Successful!" });
                }
            }

            return Unauthorized(new { Message = "Invalid UserName / Password" });
        }
    }
}
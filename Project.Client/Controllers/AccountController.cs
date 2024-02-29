using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.Client.Models;
using System.Security.Claims;

namespace Project.Client.Controllers;

public class AccountController : Controller
{
    private readonly IApiHttpClient apiHttpClient;

    public AccountController(IApiHttpClient apiHttpClient)
    {
        this.apiHttpClient = apiHttpClient;
    }

    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Logs out an user and redirects to the login page
    /// </summary>
    [HttpGet("Logout")]
    public async Task<IActionResult> Logout()
    {
        // if the user is not logged in, redirect them to the home page
        if (User?.Identity?.IsAuthenticated == false)
            return RedirectToAction("Index", "Home");
        Response.Cookies.Delete("Token");
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }

    public IActionResult Register()
    {
        RegisterDto model = new(null, null, null, null);
        return View(model);
    }

    public IActionResult Login()
    {
        LoginDto model = new(null, null);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Login(string? username, string? password)
    {
        if (string.IsNullOrWhiteSpace(username))
            ModelState.AddModelError("Username", "Username cannot be empty!");
        if (string.IsNullOrWhiteSpace(password))
            ModelState.AddModelError("Password", "Password cannot be empty!");
        var model = new LoginDto(username, password);
        if (!ModelState.IsValid)
            return View(model);
        else
        {
            var result = await apiHttpClient.PostAsync("/account/login", model);
            AuthTokenDto? responseDto = JsonConvert.DeserializeObject<AuthTokenDto>(result);

            Response.Cookies.Delete("Token");
            Response.Cookies.Delete("Role");
            Response.Cookies.Append("Token", responseDto?.Token!, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMonths(1),
                Path = "/",
                HttpOnly = true,
                Secure = true
            });
            Response.Cookies.Append("Role", responseDto?.Role!, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMonths(1),
                Path = "/",
                HttpOnly = true,
                Secure = true
            });
            // tell asp.net we are logged in
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, responseDto?.Username!),
                new Claim(ClaimTypes.NameIdentifier, responseDto?.Id.ToString()!),
                new Claim(ClaimTypes.Role, responseDto?.Role!),
                // You can add other claims as needed, for example, you might add a claim for the JWT token
                new Claim("Token", responseDto?.Token!),
            };
            ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            return Json(new { success = true });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Register(string? username, string? password, string? passwordConfirm, string? role)
    {
        if (string.IsNullOrWhiteSpace(username))
            ModelState.AddModelError("Username", "Username cannot be empty!");
        if (string.IsNullOrWhiteSpace(password))
            ModelState.AddModelError("Password", "Password cannot be empty!");
        if (string.IsNullOrWhiteSpace(passwordConfirm))
            ModelState.AddModelError("PasswordConfirm", "Password Confirm cannot be empty!");
        var model = new RegisterDto(username, password, passwordConfirm, role);
        if (!ModelState.IsValid) 
            return View(model);
        else
        {
            await apiHttpClient.PostAsync("/account/register", model);
            return Json(new { success = true });
        }
    }
}

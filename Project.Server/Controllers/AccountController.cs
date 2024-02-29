using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Backend.Common.Models;
using Project.Backend.Core;

namespace Project.Server.Controllers;

[Route("[controller]")]
[Authorize]
public class AccountController : Controller
{
    private readonly IUsersService usersService;

    public AccountController(IUsersService usersService)
    {
        this.usersService = usersService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public IActionResult Register([FromBody] RegisterDto data)
    {
        try
        {
            usersService.AddUser(data);
            return Ok(); 
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 403, title: ex.Message);
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginDto data)
    {
        try
        {
            return Ok(usersService.Login(data));
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 403, title: ex.Message);
        }
    }
}

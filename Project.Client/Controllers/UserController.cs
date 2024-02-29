using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.Client.Common.Exceptions;
using Project.Client.Common.Models;

namespace Project.Client.Controllers;

[Authorize]
public class UserController : Controller
{
    private readonly IApiHttpClient apiHttpClient;

    public UserController(IApiHttpClient apiHttpClient)
    {
        this.apiHttpClient = apiHttpClient;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ViewScoreboard()
    {
        try
        {
            var result = await apiHttpClient.GetAsync("quiz/scoreboard", HttpContext.Items["UserToken"]?.ToString());
            IEnumerable<QuizResultDto>? val = JsonConvert.DeserializeObject<IEnumerable<QuizResultDto>>(result);
            return View(val);
        }
        catch (ApiException ex)
        {
            throw;
        }
    }

    [HttpGet]
    public async Task<IActionResult> ViewTests()
    {
        try
        {
            var result = await apiHttpClient.GetAsync("quiz/userResults", HttpContext.Items["UserToken"]?.ToString());
            IEnumerable<QuizResultDto>? val = JsonConvert.DeserializeObject<IEnumerable<QuizResultDto>>(result);
            return View(val);
        }
        catch (ApiException ex)
        {
            throw;
        }
    }
}

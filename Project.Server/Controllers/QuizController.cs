using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Backend.Common.Models;
using Project.Backend.Core;
using System.Security.Claims;

namespace Project.Server.Controllers;

[Route("[controller]")]
[Authorize]
public class QuizController : Controller
{
    private readonly IQuizService quizService;

    public QuizController(IQuizService quizService)
    {
        this.quizService = quizService;
    }

    [HttpPost()]
    public async Task<IActionResult> CreateQuiz([FromBody] QuizDto data)
    {
        try
        {
            if (!TryGetUserId(out int userId))
                return Problem(statusCode: StatusCodes.Status400BadRequest, title: "No user specified!");
            return Ok(await quizService.CreateQuizAsync(data, userId));
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 403, title: ex.Message);
        }
    }

    [HttpPost("takequizz")]
    public async Task<IActionResult> TakeQuizz([FromBody] QuizTakeDto data)
    {
        try
        {
            if (!TryGetUserId(out int userId))
                return Problem(statusCode: StatusCodes.Status400BadRequest, title: "No user specified!");
            return Ok(await quizService.TakeQuizAsync(data, userId));
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 403, title: ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpPost("search/{term}")]
    public async Task<IActionResult> Search(string term)
    {
        try
        {
            return Ok(await quizService.SearchAsync(term));
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 403, title: ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            return Ok(await quizService.GetAllAsync());
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 403, title: ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            return Ok(await quizService.GetByIdAsync(id));
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 403, title: ex.Message);
        }
    }

    [HttpGet("userResults")]
    public async Task<IActionResult> GetUserResults()
    {
        try
        {
            if (!TryGetUserId(out int userId))
                return Problem(statusCode: StatusCodes.Status400BadRequest, title: "No user specified!");
            return Ok(await quizService.GetUserResultsAsync(userId));
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 403, title: ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpGet("scoreboard")]
    public async Task<IActionResult> ViewScoreboard()
    {
        try
        {
            return Ok(await quizService.GetScoreboardAsync());
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 403, title: ex.Message);
        }
    }

    [HttpPut()]
    public async Task<IActionResult> UpdateQuiz([FromBody] QuizDisplayDto data)
    {
        try
        {
            if (!TryGetUserId(out int userId))
                return Problem(statusCode: StatusCodes.Status400BadRequest, title: "No user specified!");
            return Ok(await quizService.UpdateQuizAsync(data, userId));
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 403, title: ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PublishQuiz(int id)
    {
        try
        {
            if (!TryGetUserId(out int userId))
                return Problem(statusCode: StatusCodes.Status400BadRequest, title: "No user specified!");
            return Ok(await quizService.PublishQuizAsync(id));
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 403, title: ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            return Ok(await quizService.DeleteByIdAsync(id));
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 403, title: ex.Message);
        }
    }

    private bool TryGetUserId(out int userId)
    {
        var userIdClaim = User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out userId);
    }
}

//TODO: CHECK IF QUIZ WAS TAKEN BEFORE BY THE SAME USER
//TODO: MAKE QUIZ HAVE A MAX NUMBER OF QUESTIONS
//TODO: ADD CONFIRMATION WHEN DELETING QUIZ
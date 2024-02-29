using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Backend.Common.Models;
using Project.Backend.Core;
using System.Security.Claims;

namespace Project.Server.Controllers;

[Route("[controller]")]
[Authorize]
public class QuestionController : Controller
{
    private readonly IQuestionService questionService;

    public QuestionController(IQuestionService quizService)
    {
        this.questionService = quizService;
    }

    [HttpPost()]
    public async Task<IActionResult> AddQuizQuestion([FromBody] QuizQuestionDto data)
    {
        try
        {
            if (!TryGetUserId(out int userId))
                return Problem(statusCode: StatusCodes.Status400BadRequest, title: "No user specified!");
            return Ok(await questionService.AddQuestionAsync(data, userId));
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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Project.Client.Common.Exceptions;
using Project.Client.Common.Models;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Project.Client.Controllers;

[Authorize]
public class QuestionController : Controller
{
    private readonly IApiHttpClient apiHttpClient;

    public QuestionController(IApiHttpClient apiHttpClient)
    {
        this.apiHttpClient = apiHttpClient;
    }


    [HttpGet]
    [Authorize(Roles = "Creator, Admin")]
    public async Task<IActionResult> AddQuestion(int id)
    {
        try
        {
            var quiz = await apiHttpClient.GetAsync($"quiz/{id}", HttpContext.Items["UserToken"]?.ToString());
            ViewBag.QuizId = id;
        }
        catch (ApiException ex)
        {

        }
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Creator, Admin")]
    public async Task<IActionResult> AddQuestion(QuizQuestionDto model)
    {
        ViewBag.QuizId = model?.QuizId;
        if (string.IsNullOrWhiteSpace(model.QuestionName))
            ModelState.AddModelError("QuestionName", "Question Name cannot be empty!");
        if (string.IsNullOrWhiteSpace(model.FirstOption))
            ModelState.AddModelError("FirstOption", "First Option cannot be empty!");
        if (string.IsNullOrWhiteSpace(model.SecondOption))
            ModelState.AddModelError("SecondOption", "Second Option cannot be empty!");
        if (string.IsNullOrWhiteSpace(model.CorrectAnswer))
            ModelState.AddModelError("CorrectAnswer", "Correct Answer cannot be empty!");
        if (model.CorrectAnswer?.ToLower() != model.FirstOption?.ToLower() && model.CorrectAnswer?.ToLower() != model.SecondOption?.ToLower() 
            && model.CorrectAnswer?.ToLower() != model.ThirdOption?.ToLower() && model.CorrectAnswer?.ToLower() != model.FourthOption?.ToLower())
            ModelState.AddModelError("CorrectAnswer", "Correct Answer must be one of the available options!");
        if (model.CorrectAnswerPoints <= 0)
            ModelState.AddModelError("CorrectAnswerPoints", "Correct Answer Points must be a positive number!");
        string?[] options = new[] { model.FirstOption, model.SecondOption, model.ThirdOption, model.FourthOption };
        if (options.Where(o => o == model.FirstOption).Count() > 1 || options.Where(o => o == model.SecondOption).Count() > 1
            || (options.Where(o => o == model.ThirdOption).Count() > 1 && model.ThirdOption != null) || (options.Where(o => o == model.FourthOption).Count() > 1 && model.FourthOption != null))
            ModelState.AddModelError("CorrectAnswerPoints", "Duplicate answers not allowed!");
        if (!ModelState.IsValid)
            return View(model);
        else
        {
            var result = await apiHttpClient.PostAsync("question", model, HttpContext.Items["UserToken"]?.ToString());
            QuizQuestionDto? val = JsonConvert.DeserializeObject<QuizQuestionDto>(result);
        }
        return RedirectToAction("ViewUnpublished", "Quiz");
    }
}

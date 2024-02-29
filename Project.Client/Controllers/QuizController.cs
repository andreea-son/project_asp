using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Project.Client.Common.Exceptions;
using Project.Client.Common.Models;

namespace Project.Client.Controllers;

[Authorize]
public class QuizController : Controller
{
    private readonly IApiHttpClient apiHttpClient;

    public QuizController(IApiHttpClient apiHttpClient)
    {
        this.apiHttpClient = apiHttpClient;
    }

    [HttpGet]
    [Authorize(Roles = "Creator, Admin")]
    public IActionResult CreateQuiz()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Creator, Admin")]
    public async Task<IActionResult> CreateQuiz(QuizDto model)
    {
        if (string.IsNullOrWhiteSpace(model.Name))
            ModelState.AddModelError("Name", "Name cannot be empty!");
        if (!ModelState.IsValid)
            return View(model);
        else
        {
            try
            {
                var result = await apiHttpClient.PostAsync("quiz", model, HttpContext.Items["UserToken"]?.ToString());
                QuizDto? val = JsonConvert.DeserializeObject<QuizDto>(result);
            }
            catch (ApiException ex)
            {

            }
        }
        return RedirectToAction("ViewUnpublished");
    }

    [HttpGet()]
    [AllowAnonymous]
    public async Task<IActionResult> ViewAll()
    {
        try
        {
            var result = await apiHttpClient.GetAsync("quiz", HttpContext.Items["UserToken"]?.ToString());
            IEnumerable<QuizDisplayDto>? val = JsonConvert.DeserializeObject<IEnumerable<QuizDisplayDto>>(result);
            return View(val);
        }
        catch (ApiException ex)
        {
            throw;
        }
    }

    [HttpGet()]
    [Authorize(Roles = "Creator, Admin")]
    public async Task<IActionResult> ViewUnpublished()
    {
        try
        {
            var result = await apiHttpClient.GetAsync("quiz", HttpContext.Items["UserToken"]?.ToString());
            IEnumerable<QuizDisplayDto>? val = JsonConvert.DeserializeObject<IEnumerable<QuizDisplayDto>>(result);
            return View(val);
        }
        catch (ApiException ex)
        {
            throw;
        }
    }

    [HttpPost()]
    public async Task<IActionResult> SearchQuiz(string term)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(term))
                return RedirectToAction("Index", "Home");
            var result = await apiHttpClient.PostAsync($"quiz/search/{term}", HttpContext.Items["UserToken"]?.ToString());
            IEnumerable<QuizDisplayDto>? val = JsonConvert.DeserializeObject<IEnumerable<QuizDisplayDto>>(result);
            return View(val);
        }
        catch (ApiException ex)
        {
            throw;
        }
    }

    [HttpGet()]
    [Authorize(Roles = "Creator, Admin")]
    public async Task<IActionResult> UpdateQuiz(int id)
    {
        var result = await apiHttpClient.GetAsync($"quiz/{id}", HttpContext.Items["UserToken"]?.ToString());
        QuizUpdateDto? val = JsonConvert.DeserializeObject<QuizUpdateDto>(result);
        return View(val);
    }

    [HttpPost()]
    [Authorize(Roles = "Creator, Admin")]
    public async Task<IActionResult> UpdateQuiz(QuizDisplayDto model)
    {
        if (string.IsNullOrWhiteSpace(model.Name))
            ModelState.AddModelError("Name", "Name cannot be empty!");
        if (!ModelState.IsValid)
        {
            QuizUpdateDto returnModel = new QuizUpdateDto(model.Id, model.Name, model.Category, model.Description);
            return View(returnModel);
        }
        else
        {
            try
            {
                var result = await apiHttpClient.PutAsync($"quiz", model, HttpContext.Items["UserToken"]?.ToString());
                QuizDto? val = JsonConvert.DeserializeObject<QuizDto>(result);
            }
            catch (ApiException ex)
            {

            }
            return RedirectToAction("ViewUnpublished");
        }
    }

    [Authorize(Roles = "Creator, Admin")]
    public async Task<IActionResult> DeleteQuiz(int id, string view, string controller)
    {
        var result = await apiHttpClient.DeleteAsync($"quiz/{id}", HttpContext.Items["UserToken"]?.ToString());
        IEnumerable<QuizDisplayDto>? val = JsonConvert.DeserializeObject<IEnumerable<QuizDisplayDto>>(result);
        return RedirectToAction(view, controller);
    }

    [Authorize(Roles = "Creator, Admin")]
    public async Task<IActionResult> PublishQuiz(int id)
    {
        var result = await apiHttpClient.PutAsync($"quiz/{id}", id, HttpContext.Items["UserToken"]?.ToString());
        QuizDto? val = JsonConvert.DeserializeObject<QuizDto>(result);
        return RedirectToAction("viewall");
    }

    [HttpGet()]
    public async Task<IActionResult> StartQuiz(int id)
    {
        var result = await apiHttpClient.GetAsync($"quiz/{id}", HttpContext.Items["UserToken"]?.ToString());
        QuizTakeDto? quiz = JsonConvert.DeserializeObject<QuizTakeDto>(result);
        return View(quiz);
    }

    [HttpPost]
    public async Task<IActionResult> StartQuiz(QuizTakeDto model)
    {
        var result = await apiHttpClient.PostAsync($"quiz/takequizz", model, HttpContext.Items["UserToken"]?.ToString());
        var val = JsonConvert.DeserializeObject<QuizResultDto>(result);
        return View("quizresult", val);
    }
}
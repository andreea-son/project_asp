using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.Client.Common.Models;
using Project.Client.Models;
using System.Diagnostics;

namespace Project.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly IApiHttpClient apiHttpClient;
        
        public HomeController(IApiHttpClient apiHttpClient)
        {
            this.apiHttpClient = apiHttpClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var result = await apiHttpClient.GetAsync("quiz", HttpContext.Items["UserToken"]?.ToString());
                IEnumerable<QuizDisplayDto>? val = JsonConvert.DeserializeObject<IEnumerable<QuizDisplayDto>>(result);
                return View(val);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
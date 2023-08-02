using CBD.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace CBD.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ImportJSON()
        {
            return View();
        }

        [HttpPost]
        public IActionResult BuildImport(IFormFile jsonFile, string jsonData)
    {
            // Check if a file was uploaded and use its content
            if (jsonFile != null && jsonFile.Length > 0)
            {
                using (var reader = new StreamReader(jsonFile.OpenReadStream()))
                {
                    jsonData = reader.ReadToEnd();
                }
            }


            // Step 2: Deserialize JSON data into the CharBuild.Rootobject class
            CharBuild.Build charBuildData = JsonConvert.DeserializeObject < CharBuild.Build>(jsonData);

            // Step 3: Pass the data to the view
            return View(charBuildData);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
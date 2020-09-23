using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using professionaltranslator.net.Models;
using Microsoft.AspNetCore.Authorization;

namespace professionaltranslator.net.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class HomeController : Bases.Mvc
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var modelImage = await Repository.Image.Get(Guid.Parse("4AEE5647-691A-4C65-98B2-0AE965B67405"));
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using professionaltranslator.net.Models;
using Microsoft.AspNetCore.Authorization;
using repository = professionaltranslator.net.Repository;

namespace admin.professionaltranslator.net.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class HomeController : Bases.Mvc
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SiteSettings _configuration;

        public HomeController(SiteSettings configuration, ILogger<HomeController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public async Task<IActionResult> Index()
        {
            var modelImage = await repository.Page.LocalizedItem(_configuration.Site, "About", _configuration.Culture);
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
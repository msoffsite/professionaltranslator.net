﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace web.professionaltranslator.net.ViewComponents
{
    public class Loading : ViewComponent
    {
        internal SiteSettings Configuration;

        public Loading(SiteSettings configuration)
        {
            Configuration = configuration;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IViewComponentResult> InvokeAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return View();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.ProfessionalTranslator.Net;

namespace web.professionaltranslator.net.Areas.Admin.Pages
{
    [Authorize(Roles = "Administrator")]
    public class Base : net.Pages.Base
    {
        internal AdminPortfolioSettings AdminPortfolioSettings;

        internal Area Admin = Area.Admin;
    }
}

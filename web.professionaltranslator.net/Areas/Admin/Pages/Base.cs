using Microsoft.AspNetCore.Authorization;
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

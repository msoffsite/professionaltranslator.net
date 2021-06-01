using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Repository.ProfessionalTranslator.Net;
using Data = Repository.ProfessionalTranslator.Net.Subscriber;
using Subscriber = Models.ProfessionalTranslator.Net.Subscriber;

namespace web.professionaltranslator.net.Areas.Admin.Pages
{
    public class SubscribersModel : Base
    {
        private const string PageName = "ManageSubscribers";

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        public int Count { get; set; } = -1;

        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, SiteSettings.AdminSubscriberPagingSize));

        public bool ShowPrevious => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;
        public bool ShowFirst => CurrentPage != 1;
        public bool ShowLast => CurrentPage != TotalPages;

        public string EmailAddresses { get; set; } = string.Empty;

        public List<Subscriber> List { get; set; }

        public SubscribersModel(SiteSettings siteSettings)
        {
            SiteSettings = siteSettings;
        }

        public async Task<IActionResult> OnGet()
        {
            Item = await new Base().Get(SiteSettings, Admin, PageName);
            
            List = await Data.List(SiteSettings.Site, Area.Blog, (CurrentPage - 1),
                SiteSettings.AdminSubscriberPagingSize);

            var sbSubscribers = new StringBuilder();

            List<Subscriber> subscribers = await Data.List(SiteSettings.Site, Area.Blog);
            foreach (Subscriber subscriber in subscribers)
            {
                sbSubscribers.Append(subscriber.EmailAddress);
                sbSubscribers.Append("; ");
            }

            EmailAddresses = sbSubscribers.ToString();
            
            Count = await Data.PagingCount(SiteSettings.Site, Area.Blog);

            return Item == null ? NotFound() : (IActionResult)Page();
        }

        public async Task<ActionResult> OnPostDeleteSubscriber(string emailAddress)
        {
            Result result;
            try
            {
                result = await Data.Delete(SiteSettings.Site, emailAddress);
                if (result.Status == ResultStatus.Succeeded)
                {
                    result = new Result(ResultStatus.Succeeded, $"{emailAddress} has been unsubscribed.", Guid.Empty);
                }
            }
            catch (System.Exception ex)
            {
                result = new Result(ResultStatus.Failed, ex.Message, Guid.Empty);
            }

            return new JsonResult(result);
        }
    }
}
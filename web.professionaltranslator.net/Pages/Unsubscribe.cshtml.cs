using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repository.ProfessionalTranslator.Net;
using web.professionaltranslator.net.Extensions;
using web.professionaltranslator.net.Models;

using SubscriberModel = Models.ProfessionalTranslator.Net.Subscriber;
using Data = Repository.ProfessionalTranslator.Net.Subscriber;

using Exception = System.Exception;

namespace web.professionaltranslator.net.Pages
{
    public class UnsubscribeModel : Base
    {
        public UnsubscribeModel(SiteSettings siteSettings)
        {
            SiteSettings = siteSettings;
        }

        public async Task<IActionResult> OnGet()
        {
            Item = await new Base().Get(SiteSettings, Area.Root, "Unsubscribe");
            return Item == null ? NotFound() : (IActionResult)Page();
        }

        public async Task<ActionResult> OnPostSend(string emailAddress)
        {
            Result result;
            try
            {
                SubscriberModel subscriber = await Data.Item(SiteSettings.Site, Area.Blog, emailAddress);
                if (subscriber == null)
                {
                    throw new NullReferenceException($"No subscriber found for {emailAddress}.");
                }

                result = await Data.Delete(SiteSettings.Site, subscriber.Id);
                if (result.Status == ResultStatus.Failed)
                {
                    return new JsonResult(result);
                }
                result = new Result(ResultStatus.Succeeded, $"{emailAddress} has been unsubscribed.", Guid.Empty);
            }
            catch (Exception ex)
            {
                result = new Result(ResultStatus.Failed, ex.Message, Guid.Empty);
            }

            return new JsonResult(result);
        }
    }
}

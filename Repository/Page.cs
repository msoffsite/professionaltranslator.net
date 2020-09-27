using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using professionaltranslator.net.Models;
using dbRead = professionaltranslator.net.Repository.DatabaseOperations.dbo.Read.Page;
using dbLocalizedRead = professionaltranslator.net.Repository.DatabaseOperations.Localization.Read.Page;

namespace professionaltranslator.net.Repository
{
    public class Page
    {
        public static async Task<Models.Page> Item(string site, string name)
        {
            try
            {
                if ((string.IsNullOrEmpty(site)) || (string.IsNullOrEmpty(name))) return null;
                Tables.dbo.Page page = await dbRead.Item(site, name);
                if (page == null) return null;
                Models.Image image = await Image.Item(page.ImageId);
                List<Tables.Localization.Page> localizedList = await dbLocalizedRead.List(page.Id);
                var output = new Models.Page
                {
                    Id = page.Id,
                    CanHaveImage = page.CanHaveImage,
                    IsService = page.IsService,
                    Name = page.Name,
                    Image = image,
                    Localization = localizedList.Select(n => new Models.Localized.Page
                    {
                        Lcid = n.Lcid,
                        Title = n.Title,
                        Html = n.Html
                    }).ToList()
                };
                return output;
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
                return null;
            }
        }
    }
}

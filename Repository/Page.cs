using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using dbLocalizedRead = professionaltranslator.net.Repository.DatabaseOperations.dbo.Read.Localized.Page;

namespace professionaltranslator.net.Repository
{
    public class Page
    {
        public static async Task<Models.Localized.Page> LocalizedItem(string site, string name, string culture)
        {
            try
            {
                return await dbLocalizedRead.Item(site, name, culture);
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
                return null;
            }
        }
    }
}

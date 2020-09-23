using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using dbLocalizedRead = professionaltranslator.net.Repository.DatabaseOperations.dbo.Read.Localized.Page;

namespace professionaltranslator.net.Repository
{
    internal class Page
    {
        internal static async Task<Models.Localized.Page> LocalizedItem(string name, string culture)
        {
            try
            {
                return await dbLocalizedRead.Item(name, culture);
            }
            catch
            {
                return null;
            }
        }
    }
}

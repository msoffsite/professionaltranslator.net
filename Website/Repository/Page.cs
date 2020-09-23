using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using dbLocalizedRead = professionaltranslator.net.Repository.DatabaseOperations.dbo.Read.Localized.Page;

namespace professionaltranslator.net.Repository
{
    internal class Page
    {
        internal static Models.Localized.Page LocalizedItem(string name, string culture)
        {
            try
            {
                return dbLocalizedRead.Item(name, culture);
            }
            catch
            {
                return null;
            }
        }
    }
}

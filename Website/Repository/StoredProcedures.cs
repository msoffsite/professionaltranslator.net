using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace professionaltranslator.net.Repository
{
    internal class StoredProcedures
    {
        internal class Dbo
        {
            internal class Read
            {
                internal class Image
                {
                    internal static readonly string ItemById = "[dbo].[GetImage]";
                }

                internal class Page
                {
                    internal class Localized
                    {
                        internal static readonly string ItemByEnumCulture = "[dbo].[GetLocalizedPage]";
                    }
                }
            }
        }
    }
}

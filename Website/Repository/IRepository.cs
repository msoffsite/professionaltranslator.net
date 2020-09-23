using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace professionaltranslator.net.Repository
{
    interface IRepository
    {
        T Get<T>(DbDataAdapter dbDataAdapter);
    }
}

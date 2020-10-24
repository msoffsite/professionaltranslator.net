using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Repository.ProfessionalTranslator.Net.DatabaseOperations
{
    internal class Base
    {
        internal readonly SqlConnection SqlConnection;

        internal Base()
        {
            var connectionStrings = new ConnectionStrings();
            SqlConnection = new SqlConnection(connectionStrings.SqlServer);
        }
    }
}

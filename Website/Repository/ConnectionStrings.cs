using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace professionaltranslator.net.Repository
{
    internal class ConnectionStrings
    {
        private readonly IConfiguration _configuration;

        //internal string SqlServer => _configuration["SqlServerConnection"];
        internal string SqlServer => "Server=DESKTOP-CP3USJ6\\SQL2K8R2;Database=professionaltranslator.net;User Id=cg_webuser;Password=$2020$Sept;MultipleActiveResultSets=true";

        private ConnectionStrings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        internal ConnectionStrings() 
        {
        }
    }

}

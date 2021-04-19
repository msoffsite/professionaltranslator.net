using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.ProfessionalTranslator.Net.Tables.dbo
{
    public class Client : Models.ProfessionalTranslator.Net.Client
    {
        public new Guid Id { get; set; }
        public Guid SiteId { get; set; }

        public Client() { }
    }
}

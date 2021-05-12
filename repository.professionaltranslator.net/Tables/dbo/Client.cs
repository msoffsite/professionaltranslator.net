using System;

namespace Repository.ProfessionalTranslator.Net.Tables.dbo
{
    public class Client : Models.ProfessionalTranslator.Net.Client
    {
        public Guid SiteId { get; set; }
    }
}

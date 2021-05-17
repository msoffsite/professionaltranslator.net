using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.ProfessionalTranslator.Net.Tables.dbo
{
    public class Subscriber : Models.ProfessionalTranslator.Net.Subscriber
    {
        public Guid SiteId { get; set; }
        public int AreaId { get; set; }
    }
}

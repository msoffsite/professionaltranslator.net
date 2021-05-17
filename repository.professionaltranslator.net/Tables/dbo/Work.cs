using System;

namespace Repository.ProfessionalTranslator.Net.Tables.dbo
{
    public class Work : Models.ProfessionalTranslator.Net.Work
    {
        public Guid SiteId { get; set; }
        public Guid CoverId { get; set; }
        public new DateTime DateCreated { get; set; }
    }
}

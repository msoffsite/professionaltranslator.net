using System;

namespace Repository.ProfessionalTranslator.Net.Tables.dbo
{
    public class Work : Models.Professionaltranslator.Net.Work
    {
        public new Guid Id { get; set; }
        public Guid SiteId { get; set; }
        public Guid CoverId { get; set; }
        public new DateTime DateCreated { get; set; }
    }
}

using System;

namespace Repository.ProfessionalTranslator.Net.Tables.dbo
{
    public class Page : Models.ProfessionalTranslator.Net.Page
    {
        public new Guid Id { get; set; }
        public Guid SiteId { get; set; }
        public Guid? ImageId { get; set; }
    }
}

using System;

namespace Repository.ProfessionalTranslator.Net.Tables.dbo
{
    public class Page : Models.ProfessionalTranslator.Net.Page
    {
        public Guid SiteId { get; set; }
        public int AreaId { get; set; }
        public Guid? ImageId { get; set; }
    }
}

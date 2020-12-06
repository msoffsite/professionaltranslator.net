using System;

namespace Repository.ProfessionalTranslator.Net.Tables.dbo
{
    public class Image : Models.ProfessionalTranslator.Net.Image
    {
        public new Guid Id { get; set; }
        public Guid SiteId { get; set; }

        public Image() {}
    }
}

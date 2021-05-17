using System;

namespace Repository.ProfessionalTranslator.Net.Tables.dbo
{
    public class Testimonial : Models.ProfessionalTranslator.Net.Testimonial
    {
        public Guid SiteId { get; set; }
        public Guid WorkId { get; set; }
        public Guid PortraitImageId { get; set; }
        public new DateTime DateCreated { get; set; }
    }
}

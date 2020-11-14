using System;

namespace Repository.ProfessionalTranslator.Net.Tables.dbo
{
    public class Testimonial : Models.Professionaltranslator.Net.Testimonial
    {
        public new Guid Id { get; set; }
        public Guid SiteId { get; set; }
        public Guid WorkId { get; set; }
        public Guid PortraitImageId { get; set; }
        public new DateTime DateCreated { get; set; }
    }
}

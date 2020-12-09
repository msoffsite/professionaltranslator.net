using System;

namespace Repository.ProfessionalTranslator.Net.Tables.Log
{
    public class Inquiry : Models.ProfessionalTranslator.Net.Log.Inquiry
    {
        public new Guid Id { get; set; }
        public Guid SiteId { get; set; }
        public new DateTime DateCreated { get; set; }
    }
}

using System;

namespace Repository.ProfessionalTranslator.Net.Tables.Log
{
    public class Exception : Models.ProfessionalTranslator.Net.Log.Exception
    {
        public new Guid Id { get; set; }
        public Guid SiteId { get; set; }
        public new DateTime DateCreated { get; set; }
    }
}

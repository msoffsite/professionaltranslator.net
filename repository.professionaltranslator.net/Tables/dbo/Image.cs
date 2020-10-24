using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.ProfessionalTranslator.Net.Tables.dbo
{
    public class Image : Models.Professionaltranslator.Net.Image
    {
        public new Guid Id { get; set; }
        public Guid SiteId { get; set; }

        public Image() {}
    }
}

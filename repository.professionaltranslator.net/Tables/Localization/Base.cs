using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.ProfessionalTranslator.Net.Tables.Localization
{
    public class Base
    {
        public Guid Id { get; set; }
        public int Lcid { get; set; }
        public string Html { get; set; }
    }
}

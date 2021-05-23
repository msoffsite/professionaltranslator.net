using System;
using System.Collections.Generic;
using Models.ProfessionalTranslator.Net.Localized;

namespace Models.ProfessionalTranslator.Net
{
    public class Page : Base
    {
        public string Name { get; set; }
        public List<Localized.Page> Contents { get; set; }
        public List<Localized.Pages.Quote> Quotes { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastModified { get; set; }
    }
}

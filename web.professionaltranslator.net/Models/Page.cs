using System.Collections.Generic;
using Models.ProfessionalTranslator.Net.Localized;
using Base = Models.ProfessionalTranslator.Net.Page;

namespace web.professionaltranslator.net.Models
{
    public class Page : Base
    {
        public string Title { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }
        public Enumeration.HeaderType HeaderType { get; set; }
#pragma warning disable IDE0051 // Remove unused private members
        private new List<global::Models.ProfessionalTranslator.Net.Localized.Pages.Quote> Contents { get; set; }
#pragma warning restore IDE0051 // Remove unused private members
    }
}

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
        // ReSharper disable once UnusedMember.Local
        private new List<global::Models.ProfessionalTranslator.Net.Localized.Page> Bodies { get; set; }
        // ReSharper disable once UnusedMember.Local
        private new List<PageHeader> Headers { get; set; }
    }
}

using System.Collections.Generic;
using Upload = Models.ProfessionalTranslator.Net.Upload.Client;

namespace web.professionaltranslator.net.Models
{
    public class Inquiry
    {
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string TranslationType { get; set; }
        public string TranslationDirection { get; set; }
        public string SubjectMatter { get; set; }
        public int WordCount { get; set; }
        public string Message { get; set; }
        public List<string> Uploads { get; set; }
    }
}

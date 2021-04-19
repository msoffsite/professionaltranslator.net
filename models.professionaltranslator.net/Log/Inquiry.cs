using System;
using System.Collections.Generic;

namespace Models.ProfessionalTranslator.Net.Log
{
    public class Inquiry : Base
    {
        public Guid ClientId { get; set; }
        public string TranslationType { get; set; }
        public string TranslationDirection { get; set; }
        public string SubjectMatter { get; set; }
        public int WordCount { get; set; }
        public string Message { get; set; }
        public List<Upload.Client> ClientUploads { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}

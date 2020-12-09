using System;

namespace Models.ProfessionalTranslator.Net.Log
{
    public class Inquiry : Base
    {
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string Title { get; set; }
        public string TranslationType { get; set; }
        public string Genre { get; set; }
        public int WordCount { get; set; }
        public string Message { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}

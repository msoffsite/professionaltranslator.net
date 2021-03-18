using System;

namespace Models.ProfessionalTranslator.Net
{
    public class Work : Base
    {
        public Image Cover { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public string Href { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Display { get; set; }
    }
}

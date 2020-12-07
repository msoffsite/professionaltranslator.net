using System;
using System.Collections.Generic;

namespace Models.ProfessionalTranslator.Net
{
    public class Testimonial : Base
    {
        public Work Work { get; set; }
        public Image Portrait { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public DateTime? DateCreated { get; set; }
        public bool Approved { get; set; }
        public List<Localized.Testimonial> Bodies { get; set; }
    }
}

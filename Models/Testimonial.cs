using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using professionaltranslator.net.Models;

namespace professionaltranslator.net.Models
{
    public class Testimonial : Base
    {
        public Work Work { get; set; }
        public Image Portrait { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Display { get; set; }
        public List<Localized.Testimonial> Localization { get; set; }
    }
}

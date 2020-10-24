using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Professionaltranslator.Net;

namespace Models.Professionaltranslator.Net
{
    public class Work : Base
    {
        public Image Cover { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public string Href { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Display { get; set; }
        public string TestimonialLink { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace professionaltranslator.net.Models.Localized
{
    public class Testimonial : Models.Testimonial
    {
        public int Lcid { get; set; }
        public string Html { get; set; }
    }
}

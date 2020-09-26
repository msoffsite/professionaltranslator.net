using System;
using System.Collections.Generic;
using System.Text;

namespace professionaltranslator.net.Repository.Tables.dbo
{
    public class Work : Base
    {
        public Guid CoverId { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public string Href { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Display { get; set; }
        public string TestimonialLink { get; set; }
    }
}

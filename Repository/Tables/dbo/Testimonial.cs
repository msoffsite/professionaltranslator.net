using System;
using System.Collections.Generic;
using System.Text;

namespace professionaltranslator.net.Repository.Tables.dbo
{
    public class Testimonial : Base
    {
        public Guid SiteId { get; set; }
        public Guid WorkId { get; set; }
        public Guid PortraitImageId { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Display { get; set; }
    }
}

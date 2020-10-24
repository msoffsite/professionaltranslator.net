using System;
using System.Collections.Generic;
using System.Text;

namespace professionaltranslator.net.Repository.Tables.dbo
{
    public class Testimonial : Models.Testimonial
    {
        public new Guid Id { get; set; }
        public Guid SiteId { get; set; }
        public Guid WorkId { get; set; }
        public Guid PortraitImageId { get; set; }
        public new DateTime DateCreated { get; set; }
    }
}

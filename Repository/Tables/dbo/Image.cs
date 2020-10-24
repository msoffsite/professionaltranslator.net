using System;
using System.Collections.Generic;
using System.Text;

namespace professionaltranslator.net.Repository.Tables.dbo
{
    public class Image : Models.Image
    {
        public new Guid Id { get; set; }
        public Guid SiteId { get; set; }

        public Image() {}
    }
}

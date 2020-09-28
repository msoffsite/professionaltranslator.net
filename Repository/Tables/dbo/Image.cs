using System;
using System.Collections.Generic;
using System.Text;

namespace professionaltranslator.net.Repository.Tables.dbo
{
    public class Image : Base
    {
        public Guid SiteId { get; set; }
        public string Path { get; set; }

        public Image() {}
    }
}

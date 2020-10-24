using System;
using System.Collections.Generic;
using System.Text;

namespace professionaltranslator.net.Repository.Tables.dbo
{
    public class Page : Models.Page
    {
        public new Guid Id { get; set; }
        public Guid SiteId { get; set; }
        public Guid? ImageId { get; set; }
    }
}

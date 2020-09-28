using System;
using System.Collections.Generic;
using System.Text;

namespace professionaltranslator.net.Repository.Tables.dbo
{
    public class Page : Base
    {
        public Guid SiteId { get; set; }
        public string Name { get; set; }
        public bool IsService { get; set; }
        public bool CanHaveImage { get; set; }
        public Guid? ImageId { get; set; }
    }
}

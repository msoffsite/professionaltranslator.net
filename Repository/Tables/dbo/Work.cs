using System;
using System.Collections.Generic;
using System.Text;

namespace professionaltranslator.net.Repository.Tables.dbo
{
    public class Work : Models.Work
    {
        public new Guid Id { get; set; }
        public Guid SiteId { get; set; }
        public Guid CoverId { get; set; }
        public new DateTime DateCreated { get; set; }
    }
}

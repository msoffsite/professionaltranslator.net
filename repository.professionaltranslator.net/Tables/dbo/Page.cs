﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.ProfessionalTranslator.Net.Tables.dbo
{
    public class Page : Models.Professionaltranslator.Net.Page
    {
        public new Guid Id { get; set; }
        public Guid SiteId { get; set; }
        public Guid? ImageId { get; set; }
    }
}
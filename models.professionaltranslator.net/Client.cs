﻿using System.Collections.Generic;

namespace Models.ProfessionalTranslator.Net
{
    public class Client : Base
    {
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public List<Upload.Client> Uploads { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ProfessionalTranslator.Net.Upload
{
    public class Client : Base
    {
        public string OriginalFilename { get; set; }
        public string GeneratedFilename { get; set; }
    }
}

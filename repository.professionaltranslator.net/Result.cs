using System;
using System.Collections.Generic;
using System.Text;
using Repository.ProfessionalTranslator.Net;

namespace Repository.Professionaltranslator.Net
{
    public class Result
    {
        public SaveStatus Status { get; set; }
        public List<string> Messages { get; set; }
    }
}

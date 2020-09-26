using System;
using System.Data;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using professionaltranslator.net.Models;

namespace professionaltranslator.net.Models
{
    public class Image : Base
    {
        public string Path { get; set; }

        public Image() {}
    }
}

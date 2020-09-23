using System;
using System.Data;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace professionaltranslator.net.Models
{
    public class Image
    {
        public Guid? Id { get; set; }
        public string Path { get; set; }

        public Image() {}
    }
}

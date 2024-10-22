using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Pregnancy
{
    public class Image
    {
        public string? ImageUrl { get; set; } 
        public Guid? ImageGuid { get; set; }
        public string? ImageVersion { get; set; }
        public string? ImageType { get; set; }
    }
}

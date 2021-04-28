using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SortexAdminV._1.Models
{
    public class Brand
    {
        public int Id { get; set; }
        public string Manufacturer { get; set; }
        public string Gender { get; set; }
        public string Classification { get; set; }
        public List<BrandTagMM> BrandTags { get; set; }
        public List<BrandImage> BrandImages { get; set; }
    }
}

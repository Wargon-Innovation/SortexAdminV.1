using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SortexAdminV._1.Models
{
    public class BrandImage
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public string ImageDescription { get; set; }

        public string FilePath { get; set; }

        public int BrandId { get; set; }
        public Brand Brand { get; set; }
    }
}

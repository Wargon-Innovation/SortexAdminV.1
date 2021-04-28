using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SortexAdminV._1.Models
{
    public class BrandTagMM
    {
        public int Id { get; set; }

        public int BrandId { get; set; }
        public Brand Brand { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}

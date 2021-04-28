using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SortexAdminV._1.Models
{
    public class Trend
    {
        public int Id { get; set; }
        public string Season { get; set; }
        public string Description { get; set; }
        public List<TrendImageMM> TrendImages { get; set; }

    }
}

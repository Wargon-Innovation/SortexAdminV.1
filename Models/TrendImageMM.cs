using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SortexAdminV._1.Models
{
    public class TrendImageMM
    {
        public int Id { get; set; }

        public int TrendId { get; set; }
        public Trend Trend { get; set; }

        public int TrendImageId { get; set; }
        public TrendImage TrendImage { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SortexAdminV._1.ViewModels;

namespace SortexAdminV._1.ViewModels
{
    public class TrendViewModel
    {
        public int Id { get; set; }
        public string Season { get; set; }
        public string Description { get; set; }

        public List<String> TrendImages = new List<string>();
    }
}

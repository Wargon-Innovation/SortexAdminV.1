using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SortexAdminV._1.ViewModels
{
    public class BrandUploadViewModel
    {
        public int Id { get; set; }
        public string Manufacturer { get; set; }
        public string Gender { get; set; }
        public string Classification { get; set; }
        public int NumberOfImages { get; set; }
        public string Tags { get; set; }
    }
}

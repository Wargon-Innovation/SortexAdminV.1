using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SortexAdminV._1.Models
{
    public class WargonBrands
    {
        [Key]
        public string Märke { get; set; }
        public string Kategori { get; set; }
    }
}

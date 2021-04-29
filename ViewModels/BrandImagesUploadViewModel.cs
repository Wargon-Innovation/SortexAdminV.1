using Microsoft.AspNetCore.Http;
using SortexAdminV._1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SortexAdminV._1.ViewModels
{
    public class BrandImagesUploadViewModel
    {
        public int Id { get; set; }
        public IFormFile Image { get; set; }
        public string FilePath { get; set; }
        public int BrandId { get; set; }
    }
}

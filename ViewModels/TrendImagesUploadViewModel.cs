using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SortexAdminV._1.ViewModels
{
    public class TrendImagesUploadViewModel
    {
        public int Id { get; set; }
        public IFormFile Image { get; set; }
        public string FilePath { get; set; }

        public int TrendId { get; set; }

        public int NumberOfImages { get; set; }

        //public List<IFormFile> images = new List<IFormFile>();
    }
}

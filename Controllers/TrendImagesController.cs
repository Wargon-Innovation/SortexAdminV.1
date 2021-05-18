using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SortexAdminV._1.Models;
using SortexAdminV._1.ViewModels;

namespace SortexAdminV._1.Controllers
{
    public class TrendImagesController : Controller
    {
        private readonly SortexDBContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly INotyfService _notyf;

        public TrendImagesController(SortexDBContext context, IWebHostEnvironment environment, INotyfService notyf)
        {
            _context = context;
            _environment = environment;
            _notyf = notyf;
        }

        // GET: TrendImages
        public async Task<IActionResult> Index()
        {
            return View(await _context.TrendImages.ToListAsync());
        }

        // GET: TrendImages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trendImage = await _context.TrendImages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trendImage == null)
            {
                return NotFound();
            }

            return View(trendImage);
        }
        public IActionResult AddImages(int id)
        {
            var trend = _context.Trends.FirstOrDefaultAsync(m => m.Id == id);
            var trendView = new TrendViewModel();
            trendView.Id = trend.Id;
            return View(trendView);
        }
        // GET: TrendImages/Create
        public IActionResult Create(TrendViewModel trendView)
        {
            ViewBag.TrendId = trendView.Id;
            ViewBag.NumberOfImages = trendView.NumberOfImages;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IEnumerable<IFormFile> files, TrendImagesUploadViewModel trend)
        {
            //HÄMTA DATUM
            DateTime localDate = DateTime.Now;
            var date = localDate.ToString("yyyyMMddTHHmmssZ");

            string fileName;
            //BYT DENNA TILL DEN RIKTIGA DOMÄNEN
            string websiteURL = "http://localhost:39737/";
            //string websiteURL = "https://informatik13.ei.hv.se/SortexAdmin/";

            trend.NumberOfImages = 0;
            string path = _environment.WebRootPath + "\\Uploads\\TrendImages\\";
            foreach (var image in files)
            {
                fileName = date + image.FileName.ToLower();
                TrendImage newTrendImage = new TrendImage();

                try
                {
                    //KOLLA OM BILDMAPPEN FINNS
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    using (FileStream fileStream = System.IO.File.Create(path + fileName))
                    {
                        newTrendImage.Image = websiteURL + "Uploads/TrendImages/" + fileName;
                        newTrendImage.FilePath = path + fileName;
                        _context.Add(newTrendImage);
                        await _context.SaveChangesAsync();
                        image.CopyTo(fileStream);
                        fileStream.Flush();
                    }

                    var trendImages = await _context.TrendImages.ToListAsync();
                    var lastImage = trendImages.LastOrDefault();

                    var trendImageMM = new TrendImageMM();
                    trendImageMM.TrendId = trend.TrendId;
                    trendImageMM.TrendImageId = lastImage.Id;

                    _context.Add(trendImageMM);
                    await _context.SaveChangesAsync();
                    trend.NumberOfImages += 1;
                    
                }
                catch (Exception e)
                {
                    //TempData["Result"] = "Något gick fel";
                    ViewBag.Error = e.Message;
                    return RedirectToAction("Index");
                }
            }
            _notyf.Success("Du har lagt till " + trend.NumberOfImages + " bilder till trenden");
            return RedirectToAction("Details", "Trends", new { Id = trend.Id });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trendImage = await _context.TrendImages.FindAsync(id);
            if (trendImage == null)
            {
                return NotFound();
            }
            return View(trendImage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Image")] TrendImage trendImage)
        {
            if (id != trendImage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trendImage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrendImageExists(trendImage.Id))
                    {
                        
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(trendImage);
        }

        // GET: TrendImages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            

            var trendImages = await (from rowsTrendImages in _context.TrendImages
                                join rowsTrendImagesMM in _context.TrendImageMMs on rowsTrendImages.Id equals rowsTrendImagesMM.TrendImageId
                                join rowsTrends in _context.Trends on rowsTrendImagesMM.TrendId equals rowsTrends.Id
                                where rowsTrends.Id == id
                                select rowsTrendImages).ToListAsync();

            if (trendImages == null)
            {
                return NotFound();
            }

            ViewBag.TrendId = id;
            return View(trendImages);
        }

        // POST: TrendImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(List<int> selectedImages, int Id)
        {

            try
            {
                foreach (var imageId in selectedImages)
                {
                    var image = await (from rowsTrendImage in _context.TrendImages
                                       where rowsTrendImage.Id == imageId
                                       select rowsTrendImage).FirstOrDefaultAsync();

                    FileInfo file = new FileInfo(image.FilePath);
                    if (file.Exists)
                    {
                        _context.TrendImages.Remove(image);
                        await _context.SaveChangesAsync();
                        file.Delete();
                    }

                }
                _notyf.Success("Du har tagit bort " + selectedImages.Count + " bilder från trenden");
                return RedirectToAction("Details", "Trends", new { id = Id });
            }
            catch (Exception)
            {
                _notyf.Error("Något gick fel");
                return RedirectToAction("Index", "Trends");
            }
        }

        private bool TrendImageExists(int id)
        {
            return _context.TrendImages.Any(e => e.Id == id);
        }
    }
}

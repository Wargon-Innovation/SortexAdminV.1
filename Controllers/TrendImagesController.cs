using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public TrendImagesController(SortexDBContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
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
            //string websiteURL = "http://localhost:39737/";
            string websiteURL = "https://informatik13.ei.hv.se/SortexAdmin/";

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
                }
                catch (Exception e)
                {
                    //TempData["Result"] = "Något gick fel";
                    ViewBag.Error = e.Message;
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", "Trends");
        }

        // POST: TrendImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Image")] TrendImage trendImage)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(trendImage);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(trendImage);
        //}

        // GET: TrendImages/Edit/5
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

        // POST: TrendImages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                        return NotFound();
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

            var trendImage = await _context.TrendImages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trendImage == null)
            {
                return NotFound();
            }

            return View(trendImage);
        }

        // POST: TrendImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            
            var trendImage = await _context.TrendImages.FindAsync(id);
            _context.TrendImages.Remove(trendImage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrendImageExists(int id)
        {
            return _context.TrendImages.Any(e => e.Id == id);
        }
    }
}

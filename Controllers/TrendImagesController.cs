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
            string fileName;
            //BYT DENNA TILL DEN RIKTIGA DOMÄNEN
            string websiteURL = "http://localhost:39737/";

            string path = _environment.WebRootPath + "\\TrendImages\\";
            foreach (var image in files)
            {
                fileName = image.FileName.ToLower();
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
                        newTrendImage.Image = websiteURL + "\\TrendImages\\" + fileName;
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
                catch (Exception)
                {
                    TempData["Result"] = "Något gick fel";
                    return RedirectToAction("Index");
                }
                
            }
            return View();

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
            var trendImageMMs = await _context.TrendImageMMs.ToListAsync();
            var trendImage = await _context.TrendImageMMs.ToListAsync();

            var result = (from t1 in _context.TrendImageMMs
                          join t2 in _context.TrendImages
                          where t2. == id
                          select ProductCategory.ProductCategoryId).SingleOrDefault();

            from rader in db.Vardnadshavare
            where rader.AnvNamn == anvNamn
            select rader).FirstOrDefault();


            var trendImage = await _context.TrendImages.FindAsync(id);
            _context.TrendImages.Remove(trendImage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

            from raderEtik in db.Etik
            join raderEtikBarn in db.Barn_Etik on raderEtik.Id equals raderEtikBarn.EtikId
            join raderBarn in db.Barn on raderEtikBarn.BarnPersonnummer equals raderBarn.Personnummer
            where raderBarn.SkolId == skolId
            select raderEtik;
        }

        private bool TrendImageExists(int id)
        {
            return _context.TrendImages.Any(e => e.Id == id);
        }
    }
}

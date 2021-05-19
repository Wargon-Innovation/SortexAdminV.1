using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SortexAdminV._1.Models;
using SortexAdminV._1.ViewModels;

namespace SortexAdminV._1.Controllers
{
    [Authorize]
    public class BrandImagesController : Controller
    {
        private readonly SortexDBContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly INotyfService _notyf;

        public BrandImagesController(SortexDBContext context, IWebHostEnvironment environment, INotyfService notyf)
        {
            _context = context;
            _environment = environment;
            _notyf = notyf;
        }

        // GET: BrandImages
        public async Task<IActionResult> Index()
        {
            var sortexDBContext = _context.BrandImages.Include(b => b.Brand);
            return View(await sortexDBContext.ToListAsync());
        }

        // GET: BrandImages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brandImage = await _context.BrandImages
                .Include(b => b.Brand)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brandImage == null)
            {
                return NotFound();
            }

            return View(brandImage);
        }
        public IActionResult AddImages(int id)
        {
            var brand = _context.Brands.FirstOrDefaultAsync(m => m.Id == id);
            var brandView = new BrandUploadViewModel();
            brandView.Id = brand.Id;
            return View(brandView);
        }
        // GET: BrandImages/Create
        public IActionResult Create(BrandUploadViewModel brand)
        {
            ViewBag.BrandId = brand.Id;
            ViewBag.NumberOfImages = brand.NumberOfImages;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IEnumerable<IFormFile> files, List<string> imageDescriptions, BrandImagesUploadViewModel brandImage)
        {
            DateTime localDate = DateTime.Now;
            var date = localDate.ToString("yyyyMMddTHHmmssZ");

            //BYT DENNA TILL DEN RIKTIGA DOMÄNEN
            string websiteURL = "http://localhost:39737/";
            //string websiteURL = "https://informatik13.ei.hv.se/SortexAdmin/";


            string path = _environment.WebRootPath + "\\Uploads\\BrandImages\\";
            string fileName;
            var numberOfImages = 0;
            int imageDescCount = 0;
            foreach (var image in files)
            {
                fileName = date + image.FileName.ToLower();
                BrandImage newBrandImage = new BrandImage();
                try
                {
                    //KOLLA OM BILDMAPPEN FINNS
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    using (FileStream fileStream = System.IO.File.Create(path + fileName))
                    {
                        newBrandImage.Image = websiteURL + "Uploads/BrandImages/" + fileName;
                        newBrandImage.FilePath = path + fileName;
                        newBrandImage.BrandId = brandImage.BrandId;
                        newBrandImage.ImageDescription = imageDescriptions[imageDescCount];
                        imageDescCount++;
                        _context.Add(newBrandImage);
                        await _context.SaveChangesAsync();

                        image.CopyTo(fileStream);
                        fileStream.Flush();
                        numberOfImages += 1;
                    }
                }
                catch(Exception)
                {
                    _notyf.Error("Något gick fel");
                    return RedirectToAction("Index");
                }
            }
            _notyf.Success("Du har lagt till " +  numberOfImages + " bilder till märket");
            return RedirectToAction("Details", "Brands", new { Id = brandImage.BrandId });
        }

        // GET: BrandImages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brandImage = await _context.BrandImages.FindAsync(id);
            if (brandImage == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Id", brandImage.BrandId);
            return View(brandImage);
        }

        // POST: BrandImages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Image,BrandId")] BrandImage brandImage)
        {
            if (id != brandImage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(brandImage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrandImageExists(brandImage.Id))
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
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Id", brandImage.BrandId);
            return View(brandImage);
        }

        // GET: BrandImages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brandImages = await (from rowsBrandImages in _context.BrandImages
                                     where rowsBrandImages.BrandId == id
                                     select rowsBrandImages).ToListAsync();
            if (brandImages == null)
            {
                return NotFound();
            }
            ViewBag.BrandId = id;
            return View(brandImages);
        }

        // POST: BrandImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(List<int> selectedImages, int Id)
        {
            try
            {
                foreach (var imageId in selectedImages)
                {
                    var image = await (from rowsBrandImage in _context.BrandImages
                                       where rowsBrandImage.Id == imageId
                                       select rowsBrandImage).FirstOrDefaultAsync();

                    FileInfo file = new FileInfo(image.FilePath);
                    if (file.Exists)
                    {
                        _context.BrandImages.Remove(image);
                        await _context.SaveChangesAsync();
                        file.Delete();
                    }

                }
                _notyf.Success("Du har tagit bort " + selectedImages.Count + " bilder från märket");
                return RedirectToAction("Details", "Brands", new { id = Id });
            }
            catch (Exception)
            {
                _notyf.Error("Något gick fel");
                return RedirectToAction("Index", "Brands");
            }
            
        }

        private bool BrandImageExists(int id)
        {
            return _context.BrandImages.Any(e => e.Id == id);
        }
    }
}

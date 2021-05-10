using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SortexAdminV._1.Models;
using SortexAdminV._1.ViewModels;

namespace SortexAdminV._1.Controllers
{
    public class BrandsController : Controller
    {
        private readonly SortexDBContext _context;
        private readonly INotyfService _notyf;

        public BrandsController(SortexDBContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        // GET: Brands
        public async Task<IActionResult> Index()
        {
            return View(await _context.Brands.ToListAsync());
        }

        // GET: Brands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            List<string> brandTagList = await (from rowsTag in _context.Tags
                                            join rowsBrandTag in _context.BrandTagMMs on rowsTag.Id equals rowsBrandTag.Id
                                            where rowsBrandTag.BrandId == id
                                            select rowsTag.Value).ToListAsync();

            ViewData["tags"] = brandTagList;

            return View(brand);
        }

        // GET: Brands/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Brands/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Manufacturer,Gender,Classification,NumberOfImages,Tags")] BrandUploadViewModel brand)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    //LADDA UPP MÄRKET
                    Brand newBrand = new Brand();

                    newBrand.Manufacturer = brand.Manufacturer;
                    newBrand.Gender = brand.Gender;
                    newBrand.Classification = brand.Classification;

                    _context.Add(newBrand);
                    await _context.SaveChangesAsync();

                    //LADDA UPP ALLA TAGGAR
                    int brandId = newBrand.Id;
                    string[] tags = brand.Tags.Split(' ');
                    foreach (var tag in tags)
                    {
                        //SPARA VARJE TAGG I DATABASEN
                        Tag newTag = new Tag();
                        newTag.Value = tag;

                        _context.Add(newTag);
                        await _context.SaveChangesAsync();

                        //SPARA KOPPLINGEN
                        int tagId = newTag.Id;

                        BrandTagMM brandTagMM = new BrandTagMM();
                        brandTagMM.BrandId = brandId;
                        brandTagMM.TagId = tagId;
                        _context.Add(brandTagMM);
                        await _context.SaveChangesAsync();


                    }
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {

                }
            }
            return View(brand);
            /*
            if (ModelState.IsValid)
            {
                _context.Add(brand);
                await _context.SaveChangesAsync();
                _notyf.Success("Du har lagt till märket " + brand.Manufacturer);
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
            */
        }

        // GET: Brands/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        // POST: Brands/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Manufacturer,Gender,Classification")] Brand brand)
        {
            if (id != brand.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(brand);
                    await _context.SaveChangesAsync();
                    _notyf.Success("Du har uppdaterat märket " + brand.Manufacturer);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrandExists(brand.Id))
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
            return View(brand);
        }

        // GET: Brands/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // POST: Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //TODO: TA ÄVEN BORT ALLA TAGGAR TILLHÖRANDE DETTA MÄRKE

            var brand = await _context.Brands.FindAsync(id);
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            _notyf.Success("Du har tagit bort märket " + brand.Manufacturer);
            return RedirectToAction(nameof(Index));
        }

        private bool BrandExists(int id)
        {
            return _context.Brands.Any(e => e.Id == id);
        }
    }
}

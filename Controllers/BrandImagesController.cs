using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SortexAdminV._1.Models;

namespace SortexAdminV._1.Controllers
{
    public class BrandImagesController : Controller
    {
        private readonly SortexDBContext _context;

        public BrandImagesController(SortexDBContext context)
        {
            _context = context;
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

        // GET: BrandImages/Create
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Id");
            return View();
        }

        // POST: BrandImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Image,BrandId")] BrandImage brandImage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(brandImage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Id", brandImage.BrandId);
            return View(brandImage);
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

            var brandImage = await _context.BrandImages
                .Include(b => b.Brand)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brandImage == null)
            {
                return NotFound();
            }

            return View(brandImage);
        }

        // POST: BrandImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brandImage = await _context.BrandImages.FindAsync(id);
            _context.BrandImages.Remove(brandImage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BrandImageExists(int id)
        {
            return _context.BrandImages.Any(e => e.Id == id);
        }
    }
}

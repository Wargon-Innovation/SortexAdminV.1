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
    public class TrendImagesController : Controller
    {
        private readonly SortexDBContext _context;

        public TrendImagesController(SortexDBContext context)
        {
            _context = context;
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: TrendImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Image")] TrendImage trendImage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trendImage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trendImage);
        }

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

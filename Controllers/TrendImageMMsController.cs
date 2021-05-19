using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SortexAdminV._1.Models;

namespace SortexAdminV._1.Controllers
{
    [Authorize]
    public class TrendImageMMsController : Controller
    {
        private readonly SortexDBContext _context;

        public TrendImageMMsController(SortexDBContext context)
        {
            _context = context;
        }

        // GET: TrendImageMMs
        public async Task<IActionResult> Index()
        {
            var sortexDBContext = _context.TrendImageMMs.Include(t => t.Trend).Include(t => t.TrendImage);
            return View(await sortexDBContext.ToListAsync());
        }

        // GET: TrendImageMMs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trendImageMM = await _context.TrendImageMMs
                .Include(t => t.Trend)
                .Include(t => t.TrendImage)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trendImageMM == null)
            {
                return NotFound();
            }

            return View(trendImageMM);
        }

        // GET: TrendImageMMs/Create
        public IActionResult Create()
        {
            ViewData["TrendId"] = new SelectList(_context.Trends, "Id", "Id");
            ViewData["TrendImageId"] = new SelectList(_context.TrendImages, "Id", "Id");
            return View();
        }

        // POST: TrendImageMMs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TrendId,TrendImageId")] TrendImageMM trendImageMM)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trendImageMM);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TrendId"] = new SelectList(_context.Trends, "Id", "Id", trendImageMM.TrendId);
            ViewData["TrendImageId"] = new SelectList(_context.TrendImages, "Id", "Id", trendImageMM.TrendImageId);
            return View(trendImageMM);
        }

        // GET: TrendImageMMs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trendImageMM = await _context.TrendImageMMs.FindAsync(id);
            if (trendImageMM == null)
            {
                return NotFound();
            }
            ViewData["TrendId"] = new SelectList(_context.Trends, "Id", "Id", trendImageMM.TrendId);
            ViewData["TrendImageId"] = new SelectList(_context.TrendImages, "Id", "Id", trendImageMM.TrendImageId);
            return View(trendImageMM);
        }

        // POST: TrendImageMMs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TrendId,TrendImageId")] TrendImageMM trendImageMM)
        {
            if (id != trendImageMM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trendImageMM);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrendImageMMExists(trendImageMM.Id))
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
            ViewData["TrendId"] = new SelectList(_context.Trends, "Id", "Id", trendImageMM.TrendId);
            ViewData["TrendImageId"] = new SelectList(_context.TrendImages, "Id", "Id", trendImageMM.TrendImageId);
            return View(trendImageMM);
        }

        // GET: TrendImageMMs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trendImageMM = await _context.TrendImageMMs
                .Include(t => t.Trend)
                .Include(t => t.TrendImage)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trendImageMM == null)
            {
                return NotFound();
            }

            return View(trendImageMM);
        }

        // POST: TrendImageMMs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trendImageMM = await _context.TrendImageMMs.FindAsync(id);
            _context.TrendImageMMs.Remove(trendImageMM);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrendImageMMExists(int id)
        {
            return _context.TrendImageMMs.Any(e => e.Id == id);
        }
    }
}

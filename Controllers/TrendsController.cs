using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SortexAdminV._1.Models;
using SortexAdminV._1.ViewModels;

namespace SortexAdminV._1.Controllers
{
    public class TrendsController : Controller
    {
        private readonly SortexDBContext _context;

        public TrendsController(SortexDBContext context)
        {
            _context = context;
        }

        // GET: Trends
        public async Task<IActionResult> Index()
        {
            //HÄMTA TRE TABELLER FRÅN DB
            //SKAPA LISTA FÖR VY
            var trendList = await _context.Trends.ToListAsync();
            var trendImagesMM = await _context.TrendImageMMs.ToListAsync();
            var trendImages = await _context.TrendImages.ToListAsync();
            List<TrendViewModel> trendViewList = new List<TrendViewModel>();

            //LÄGG TILL TRENDER I VYLISTA
            foreach (var trend in trendList)
            {
                var trendView = new TrendViewModel();
                trendView.Id = trend.Id;
                trendView.Season = trend.Season;
                trendView.Description = trend.Description;

                //OM ID STÄMMER LÄGG TILL BILDER PÅ VIEW OBJEKT
                foreach (var trendImage in trendImagesMM)
                {
                    if (trend.Id == trendImage.Id)
                    {
                        foreach (var image in trendImages)
                        {

                            trendView.TrendImages.Add(image.Image);
                            
                        }
                    }
                }
                trendViewList.Add(trendView);
            }
            
            return View(trendViewList);
        }

        // GET: Trends/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trend = await _context.Trends
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trend == null)
            {
                return NotFound();
            }

            return View(trend);
        }

        // GET: Trends/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Trends/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Season,Description")] Trend trend)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trend);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trend);
        }

        // GET: Trends/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trend = await _context.Trends.FindAsync(id);
            if (trend == null)
            {
                return NotFound();
            }
            return View(trend);
        }

        // POST: Trends/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Season,Description")] Trend trend)
        {
            if (id != trend.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trend);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrendExists(trend.Id))
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
            return View(trend);
        }

        // GET: Trends/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trend = await _context.Trends
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trend == null)
            {
                return NotFound();
            }

            return View(trend);
        }

        // POST: Trends/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trend = await _context.Trends.FindAsync(id);
            _context.Trends.Remove(trend);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrendExists(int id)
        {
            return _context.Trends.Any(e => e.Id == id);
        }
    }
}

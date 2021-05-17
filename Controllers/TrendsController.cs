using System;
using System.Collections.Generic;
using System.IO;
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
    public class TrendsController : Controller
    {
        private readonly SortexDBContext _context;
        private readonly INotyfService _notyf;

        public TrendsController(SortexDBContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        // GET: Trends
        public async Task<IActionResult> Index()
        {
            //HÄMTA TRE TABELLER FRÅN DB
            //SKAPA LISTA FÖR VY
            var trendList = await _context.Trends.ToListAsync();
            List<TrendViewModel> trendViewList = new List<TrendViewModel>();

            foreach(var trend in trendList)
            {
                TrendViewModel trendView = new TrendViewModel();
                trendView.Id = trend.Id;
                trendView.Season = trend.Season;
                trendView.Description = trend.Description;

                var result = await (from rowsTrendImageMM in _context.TrendImageMMs
                                    join rowsTrendImage in _context.TrendImages on rowsTrendImageMM.TrendImageId equals rowsTrendImage.Id
                                    where rowsTrendImageMM.TrendId == trend.Id
                                    select rowsTrendImage.Image).ToListAsync();

                foreach (var image in result)
                {
                    trendView.TrendImages.Add(image);
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

            var trendView = new TrendViewModel();

            trendView.Id = trend.Id;
            trendView.Season = trend.Season;
            trendView.Description = trend.Description;

            var result = await (from rowsTrendImageMM in _context.TrendImageMMs
                                join rowsTrendImage in _context.TrendImages on rowsTrendImageMM.TrendImageId equals rowsTrendImage.Id
                                where rowsTrendImageMM.TrendId == trend.Id
                                select rowsTrendImage.Image).ToListAsync();

            foreach (var image in result)
            {
                trendView.TrendImages.Add(image);
            }


            return View(trendView);
        }

        // GET: Trends/Create
        public IActionResult CreateTrend()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public RedirectToActionResult AddImageToTrend([Bind("Id,NumberOfImages")] TrendViewModel trendView)
        {

            return RedirectToAction("Create", "TrendImages", trendView);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTrend([Bind("Id,Season,Description,NumberOfImages")] TrendViewModel trendView)
        {
            var trend = new Trend();
            trend.Season = trendView.Season;
            trend.Description = trendView.Description;
            if (ModelState.IsValid)
            {
                _context.Add(trend);
                await _context.SaveChangesAsync();
                _notyf.Success("Du har lagt till trenden " + trend.Season);
            }

            var trendList = await _context.Trends.ToListAsync();
            var lastTrend = trendList.LastOrDefault();

            trendView.Id = lastTrend.Id;

            //KOLLA OM DET INTE SKA LADDAS UPP BILDER
            if(trendView.NumberOfImages == 0)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Create", "TrendImages", trendView);

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
                    _notyf.Success("Du har uppdaterat trend " + trend.Season);
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
            // HÄMTAR ALLA TRENDIMAGE RELATERDE TILL TREND ID
            var result = await (from rowsTrendImgMM in _context.TrendImageMMs
                                join rowsTrendImg in _context.TrendImages on rowsTrendImgMM.TrendImageId equals rowsTrendImg.Id
                                where rowsTrendImgMM.TrendId == id
                                select rowsTrendImg).ToListAsync();

            foreach (var image in result)
            {
                if (image.FilePath != null)
                {
                    FileInfo file = new FileInfo(image.FilePath);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }
                _context.TrendImages.Remove(image);
            }



            var trend = await _context.Trends.FindAsync(id);
            _context.Trends.Remove(trend);
            await _context.SaveChangesAsync();
            _notyf.Success("Du har tagit bort " + trend.Season);
            return RedirectToAction(nameof(Index));
        }

        private bool TrendExists(int id)
        {
            return _context.Trends.Any(e => e.Id == id);
        }
    }
}

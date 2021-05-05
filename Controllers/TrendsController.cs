﻿using System;
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
                foreach (var imageMM in trendImagesMM)
                {
                    if (trend.Id == imageMM.TrendId)
                    {
                        foreach (var image in trendImages)
                        {
                            if (image.Id == imageMM.TrendImageId)
                            {
                                trendView.TrendImages.Add(image.Image);
                            }
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
            //HÄMTA TRE TABELLER FRÅN DB
            //SKAPA LISTA FÖR VY
            var trendList = await _context.Trends.ToListAsync();
            var trendImagesMM = await _context.TrendImageMMs.ToListAsync();
            var trendImages = await _context.TrendImages.ToListAsync();
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


            return View(trendView);
        }

        //// GET: Trends/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        // GET: Trends/Create
        public IActionResult CreateTrend()
        {
            return View();
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
            }

            var trendList = await _context.Trends.ToListAsync();
            var lastTrend = trendList.LastOrDefault();

            trendView.Id = lastTrend.Id;

            return RedirectToAction("Create", "TrendImages", trendView);

        }

        // POST: Trends/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(TrendViewModel inputView)
        //{
        //    var trend = new Trend();

        //    trend.Season = inputView.Season;

        //    trend.Description = inputView.Description;
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(trend);
        //        await _context.SaveChangesAsync();
        //    }


        //    var trendImage = new TrendImage();
        //    trendImage.Image = "Test";

        //    _context.TrendImages.Add(trendImage);
        //    await _context.SaveChangesAsync();


        //    var trendList = await _context.Trends.ToListAsync();
        //    var trendImages = await _context.TrendImages.ToListAsync();

        //    var lastTrend = trendList.LastOrDefault();
        //    var lastImage = trendImages.LastOrDefault();


        //    var trendImageMM = new TrendImageMM();
        //    trendImageMM.TrendId = lastTrend.Id;
        //    trendImageMM.TrendImageId = lastImage.Id;


        //    if (ModelState.IsValid)
        //    {
        //        _context.TrendImageMMs.Add(trendImageMM);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View(trend);
        //}

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

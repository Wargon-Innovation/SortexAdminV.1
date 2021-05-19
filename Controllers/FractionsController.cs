using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SortexAdminV._1.Models;

namespace SortexAdminV._1.Controllers
{
    [Authorize]
    public class FractionsController : Controller
    {
        private readonly SortexDBContext _context;
        private readonly INotyfService _notyf;

        public FractionsController(SortexDBContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        // GET: Fractions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Fractions.ToListAsync());
        }

        // GET: Fractions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fraction = await _context.Fractions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fraction == null)
            {
                return NotFound();
            }

            return View(fraction);
        }

        // GET: Fractions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fractions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Number")] Fraction fraction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fraction);
                await _context.SaveChangesAsync();
                _notyf.Success("Du har lagt till fraktion " + fraction.Name);
                return RedirectToAction("Details", "Fractions", new { Id = fraction.Id });
            }
            _notyf.Error("Något gick fel");
            return View(fraction);
        }

        // GET: Fractions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fraction = await _context.Fractions.FindAsync(id);
            if (fraction == null)
            {
                return NotFound();
            }
            return View(fraction);
        }

        // POST: Fractions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Number")] Fraction fraction)
        {
            if (id != fraction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fraction);
                    await _context.SaveChangesAsync();
                    _notyf.Success("Du har uppdaterat fraktion " + fraction.Name);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FractionExists(fraction.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Fractions", new { Id = fraction.Id });
            }
            _notyf.Error("Något gick fel");
            return View(fraction);
        }

        // GET: Fractions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fraction = await _context.Fractions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fraction == null)
            {
                return NotFound();
            }

            return View(fraction);
        }

        // POST: Fractions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fraction = await _context.Fractions.FindAsync(id);
            _context.Fractions.Remove(fraction);
            await _context.SaveChangesAsync();
            _notyf.Success("Du har tagit bort fraktion " + fraction.Name);
            return RedirectToAction(nameof(Index));
        }

        private bool FractionExists(int id)
        {
            return _context.Fractions.Any(e => e.Id == id);
        }
    }
}

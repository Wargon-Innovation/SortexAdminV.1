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
    public class ModeboardsController : Controller
    {
        private readonly SortexDBContext _context;
        private readonly INotyfService _notyf;

        public ModeboardsController(SortexDBContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        // GET: Modeboards
        public async Task<IActionResult> Index()
        {
            return View(await _context.Modeboards.ToListAsync());
        }

        // GET: Modeboards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modeboard = await _context.Modeboards
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modeboard == null)
            {
                return NotFound();
            }

            return View(modeboard);
        }

        // GET: Modeboards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Modeboards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Image")] Modeboard modeboard)
        {
            if (ModelState.IsValid)
            {
                _context.Add(modeboard);
                await _context.SaveChangesAsync();
                _notyf.Success("Du har lagt till fraktion " + modeboard.Name);
                return RedirectToAction("Details", "Modeboards", new { Id = modeboard.Id });
            }
            return View(modeboard);
        }

        // GET: Modeboards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modeboard = await _context.Modeboards.FindAsync(id);
            if (modeboard == null)
            {
                return NotFound();
            }
            return View(modeboard);
        }

        // POST: Modeboards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Image")] Modeboard modeboard)
        {
            if (id != modeboard.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(modeboard);
                    await _context.SaveChangesAsync();
                    _notyf.Success("Du har uppdaterat modeboard " + modeboard.Name);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModeboardExists(modeboard.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Modeboards", new { Id = modeboard.Id });
            }
            return View(modeboard);
        }

        // GET: Modeboards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modeboard = await _context.Modeboards
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modeboard == null)
            {
                return NotFound();
            }

            return View(modeboard);
        }

        // POST: Modeboards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var modeboard = await _context.Modeboards.FindAsync(id);
            _context.Modeboards.Remove(modeboard);
            await _context.SaveChangesAsync();
            _notyf.Success("Du har tagit bort modeboard " + modeboard.Name);
            return RedirectToAction(nameof(Index));
        }

        private bool ModeboardExists(int id)
        {
            return _context.Modeboards.Any(e => e.Id == id);
        }
    }
}

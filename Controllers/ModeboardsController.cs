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

namespace SortexAdminV._1.Controllers
{
    [Authorize]
    public class ModeboardsController : Controller
    {
        private readonly SortexDBContext _context;
        private readonly INotyfService _notyf;
        private readonly IWebHostEnvironment _environment;

        public ModeboardsController(SortexDBContext context, INotyfService notyf, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
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
        public async Task<IActionResult> Create(Modeboard modeboard, IFormFile file)
        {
            DateTime localDate = DateTime.Now;
            var date = localDate.ToString("yyyyMMddTHHmmssZ");

            //BYT DENNA TILL DEN RIKTIGA DOMÄNEN
            string websiteURL = "http://localhost:39737/";
            //string websiteURL = "https://informatik13.ei.hv.se/SortexAdmin/";


            string path = _environment.WebRootPath + "\\Uploads\\ModeboardImages\\";
            string fileName;

                fileName = date + file.FileName.ToLower();

                try
                {
                    //KOLLA OM BILDMAPPEN FINNS
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    using (FileStream fileStream = System.IO.File.Create(path + fileName))
                    {
                        modeboard.Image = websiteURL + "Uploads/ModeboardImages/" + fileName;
                        modeboard.FilePath = path + fileName;

                        _context.Add(modeboard);
                        await _context.SaveChangesAsync();

                        file.CopyTo(fileStream);
                        fileStream.Flush();
                    _notyf.Success("Du har lagt till modeboard " + modeboard.Name);
                    return RedirectToAction("Details", "Modeboards", new { Id = modeboard.Id });
                }
                }
                catch (Exception)
                {
                    _notyf.Error("Något gick fel");
                    return RedirectToAction("Index");
                }
            
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description, Image, FilePath")] Modeboard modeboard)
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

            FileInfo file = new FileInfo(modeboard.FilePath);
            if (file.Exists)
            {
                file.Delete();
            }
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

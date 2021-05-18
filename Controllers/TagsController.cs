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
    public class TagsController : Controller
    {
        private readonly SortexDBContext _context;

        public TagsController(SortexDBContext context)
        {
            _context = context;
        }

        // GET: Tags
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tags.ToListAsync());
        }

        // GET: Tags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
        }

        // GET: Tags/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tags/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Value")] Tag tag)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tag);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tag);
        }

        // GET: Tags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }
            return View(tag);
        }

        // POST: Tags/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Value")] Tag tag)
        {
            if (id != tag.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TagExists(tag.Id))
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
            return View(tag);
        }

        // GET: Tags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tags = await (from rowsTag in _context.Tags
                              join rowsBrandTagMM in _context.BrandTagMMs on rowsTag.Id equals rowsBrandTagMM.TagId
                              join rowsBrand in _context.Brands on rowsBrandTagMM.BrandId equals rowsBrand.Id
                              where rowsBrand.Id == id
                              select rowsTag).ToListAsync();

            if (tags == null)
            {
                return NotFound();
            }

            ViewBag.BrandId = id;
            return View(tags);
        }

        // POST: Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(List<int> selectedTags, int Id)
        {
            try
            {
                foreach (var tagId in selectedTags)
                {
                    var tag = await (from rowsTag in _context.Tags
                                     where rowsTag.Id == tagId
                                     select rowsTag).FirstOrDefaultAsync();
                    _context.Tags.Remove(tag);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Details", "Brands", new { id = Id});
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Brands");
            }
        }

        public IActionResult AddTags(int id)
        {
            BrandUploadViewModel brandView = new BrandUploadViewModel();
            brandView.Id = id;
            return View(brandView);
        }

        [HttpPost]
        public async Task<IActionResult> AddTags(BrandUploadViewModel brandTags)
        {
            try
            {
                int brandId = brandTags.Id;
                string[] tags = brandTags.Tags.Split(' ');
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
                return RedirectToAction("Details", "Brands", new { id = brandId});
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Brands");
            }
        }

        private bool TagExists(int id)
        {
            return _context.Tags.Any(e => e.Id == id);
        }
    }
}

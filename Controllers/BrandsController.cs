using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SortexAdminV._1.Models;
using SortexAdminV._1.ViewModels;

namespace SortexAdminV._1.Controllers
{
    [Authorize]
    public class BrandsController : Controller
    {
        private readonly SortexDBContext _context;
        private readonly INotyfService _notyf;

        public BrandsController(SortexDBContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        // GET: Brands
        public async Task<IActionResult> Index(string searchTag)
        {
            return View(await _context.Brands.ToListAsync());
        }



        // GET: Brands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            //HÄMTA ALLA TAGGAR
            List<string> brandTagList = await (from rowsTag in _context.Tags
                                            join rowsBrandTag in _context.BrandTagMMs on rowsTag.Id equals rowsBrandTag.Id
                                            where rowsBrandTag.BrandId == id
                                            select rowsTag.Value).ToListAsync();

            ViewData["tags"] = brandTagList;

            //HÄMTA ALLA BILDER
            var brandImages = await (from rowsImages in _context.BrandImages
                                              join rowsBrand in _context.Brands on rowsImages.BrandId equals rowsBrand.Id
                                              where rowsBrand.Id == id
                                              select rowsImages).ToListAsync();
            ViewData["images"] = brandImages;

            return View(brand);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public RedirectToActionResult AddImageToBrand([Bind("Id,NumberOfImages")] BrandUploadViewModel brandView)
        {

            return RedirectToAction("Create", "BrandImages", brandView);

        }

        // GET: Brands/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Brands/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Manufacturer,Gender,Classification,NumberOfImages,Tags")] BrandUploadViewModel brand)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    //LADDA UPP MÄRKET
                    Brand newBrand = new Brand();

                    newBrand.Manufacturer = brand.Manufacturer;
                    newBrand.Gender = brand.Gender;
                    newBrand.Classification = brand.Classification;

                    _context.Add(newBrand);
                    await _context.SaveChangesAsync();

                    //SÄTT BRANDID FÖR ATT SKICKA TILL CREATEBRANDIMAGE
                    brand.Id = newBrand.Id;

                    //LÄGG TILL STANDARD TAGGAR FÖR MANUFACTURER, GENDER, CLASSIFICATION
                    List<Tag> standardTags = new List<Tag>();
                    Tag manufacturerTag = new Tag();
                    manufacturerTag.Value = brand.Manufacturer;
                    standardTags.Add(manufacturerTag);

                    Tag genderTag = new Tag();
                    genderTag.Value = brand.Gender;
                    standardTags.Add(genderTag);

                    Tag classificationTag = new Tag();
                    classificationTag.Value = brand.Classification;
                    standardTags.Add(classificationTag);

                    foreach (var tag in standardTags)
                    {
                        _context.Add(tag);
                        await _context.SaveChangesAsync();

                        int tagId = tag.Id;

                        BrandTagMM brandTagMM = new BrandTagMM();
                        brandTagMM.BrandId = brand.Id;
                        brandTagMM.TagId = tagId;
                        _context.Add(brandTagMM);
                        await _context.SaveChangesAsync();
                    }

                    //KOLLA OM DET FINNS NÅGRA ANDRA TAGGAR
                    if (brand.Tags != null)
                    {
                        //LADDA UPP ALLA TAGGAR
                        int brandId = newBrand.Id;
                        string[] tags = brand.Tags.Split(' ');
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

                    }
                    //KOLLA OM BILDER SKA LADDAS UPP
                    if (brand.NumberOfImages <= 0 || brand.NumberOfImages == 0)
                    {
                        _notyf.Success("Du har lagt till märket " + brand.Manufacturer);
                        return RedirectToAction("Index");
                    }
                    _notyf.Success("Du har lagt till märket " + brand.Manufacturer);
                    return RedirectToAction("Create", "BrandImages", brand);
                }
                catch (Exception)
                {
                    return View(brand);
                }
            }
            return View(brand);
        }

        // GET: Brands/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        // POST: Brands/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Manufacturer,Gender,Classification")] Brand brand)
        {
            if (id != brand.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(brand);
                    await _context.SaveChangesAsync();
                    _notyf.Success("Du har uppdaterat märket " + brand.Manufacturer);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrandExists(brand.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Brands", new { Id = brand.Id });
            }
            return View(brand);
        }

        // GET: Brands/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // POST: Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //TA BORT ALLA TAGGAR TILLHÖRANDE DETTA MÄRKE
            var tags = await (from rowsTags in _context.Tags
                              join rowsBrandTags in _context.BrandTagMMs on rowsTags.Id equals rowsBrandTags.TagId
                              where rowsBrandTags.BrandId == id
                              select rowsTags).ToListAsync();

            if(tags.Count > 0 || tags != null)
            {
                foreach (var tag in tags)
                {
                    _context.Tags.Remove(tag);
                }
                await _context.SaveChangesAsync();
            }

            //TA BORT ALLA BILDER TILLHÖRANDE DETTA MÄRKE
            var images = await (from rowsImages in _context.BrandImages
                                where rowsImages.BrandId == id
                                select rowsImages).ToListAsync();

            if(images.Count > 0 || images != null)
            {
                foreach (var brandImage in images)
                {
                    if (brandImage.FilePath != null)
                    {
                        FileInfo file = new FileInfo(brandImage.FilePath);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }
                }
            }

            var brand = await _context.Brands.FindAsync(id);
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            _notyf.Success("Du har tagit bort märket " + brand.Manufacturer);
            return RedirectToAction(nameof(Index));
        }

        private bool BrandExists(int id)
        {
            return _context.Brands.Any(e => e.Id == id);
        }
        private async void AddWargonBrand()
        {
            List<WargonBrands> wgBrands = new List<WargonBrands>();
            wgBrands = _context.WargonBrands.ToList();
            

            foreach (var brand in wgBrands)
            {
                BrandUploadViewModel newBrand = new BrandUploadViewModel();
                newBrand.Manufacturer = brand.Märke;
                newBrand.Gender = "Unisex";
                //newBrand.Id = 0;
                for(int i=0; i<brand.Kategori.Length; i++)
                {
                    
                    if (brand.Kategori[i] =='L')
                    {
                        newBrand.Classification += "Lågpris" + " ";
                    }
                    else if (brand.Kategori[i] == 'M')
                    {
                        newBrand.Classification += "Medelpris" + " ";
                    }
                    else if (brand.Kategori[i] == 'H')
                    {
                        newBrand.Classification += "Högt pris" + " ";
                    }
                    else if (brand.Kategori[i] == 'E')
                    {
                        newBrand.Classification += "Exklusiv" + " ";
                    }
                    else if (brand.Kategori[i] == '?')
                    {
                        newBrand.Classification += "Kan ej hitta information om prisklass" + " ";
                    }

                }


                if (brand.Kategori.Contains("SP"))
                {
                    newBrand.Tags += "Sport" + " ";
                }
                else if (brand.Kategori.Contains("SÖ"))
                {
                    newBrand.Tags += "Sömnad - skräddat" + " ";
                }
                else if (brand.Kategori.Contains("B"))
                {
                    newBrand.Tags += "Barn" + " ";
                }

                await InsertBrands(newBrand);
                
            }
        }

        public async Task InsertBrands([Bind("Id,Manufacturer,Gender,Classification,NumberOfImages,Tags")] BrandUploadViewModel brand)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    //LADDA UPP MÄRKET
                    Brand newBrand = new Brand();

                    newBrand.Manufacturer = brand.Manufacturer;
                    newBrand.Gender = brand.Gender;
                    newBrand.Classification = brand.Classification;

                    _context.Add(newBrand);
                    await _context.SaveChangesAsync();

                    //SÄTT BRANDID FÖR ATT SKICKA TILL CREATEBRANDIMAGE
                    brand.Id = newBrand.Id;

                    //LÄGG TILL STANDARD TAGGAR FÖR MANUFACTURER, GENDER, CLASSIFICATION
                    List<Tag> standardTags = new List<Tag>();
                    Tag manufacturerTag = new Tag();
                    manufacturerTag.Value = brand.Manufacturer;
                    standardTags.Add(manufacturerTag);

                    Tag genderTag = new Tag();
                    genderTag.Value = brand.Gender;
                    standardTags.Add(genderTag);

                    Tag classificationTag = new Tag();
                    classificationTag.Value = brand.Classification;
                    standardTags.Add(classificationTag);

                    foreach (var tag in standardTags)
                    {
                        _context.Add(tag);
                        await _context.SaveChangesAsync();

                        int tagId = tag.Id;

                        BrandTagMM brandTagMM = new BrandTagMM();
                        brandTagMM.BrandId = brand.Id;
                        brandTagMM.TagId = tagId;
                        _context.Add(brandTagMM);
                        await _context.SaveChangesAsync();
                    }

                    //KOLLA OM DET FINNS NÅGRA ANDRA TAGGAR
                    if (brand.Tags != null)
                    {
                        //LADDA UPP ALLA TAGGAR
                        int brandId = newBrand.Id;
                        string[] tags = brand.Tags.Split(' ');
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

                    }
                }
                catch (Exception)
                {
                    
                }
            }
        }
    }
}

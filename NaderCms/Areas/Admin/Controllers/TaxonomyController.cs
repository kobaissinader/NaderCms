using NaderCms.Infrastructure;
using NaderCms.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NaderCms.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin,editor")]
    [Area("Admin")]
    public class TaxonomyController : Controller
    {
        private readonly NaderCmsContext context;
        public TaxonomyController(NaderCmsContext context)
        {
            this.context = context;
        }

        public async Task<ViewResult> Index()
        {
            List<Taxonomy> tax = await context.Taxonomies.ToListAsync();

            return View(tax);
        }

        // GET /admin/taxonomy/create
        public IActionResult Create() => View();

        // POST /admin/taxonomy/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Taxonomy taxonomy)
        {
            var taxonomyCheck = await context.Taxonomies.FirstOrDefaultAsync(x => x.TaxonomyName == taxonomy.TaxonomyName);
            if (taxonomyCheck != null)
                ModelState.AddModelError("", "The taxanomoy already exists.");
            
            if (ModelState.IsValid)
            {
                
                context.Add(taxonomy);
                await context.SaveChangesAsync();

                TempData["Success"] = "The taxanomoy has been added!";

                return RedirectToAction("Index");
            }

            return View(taxonomy);
        }

        // GET /admin/taxonomy/edit/id

        public async Task<IActionResult> Edit(int Taxonomyid)
        {
            Taxonomy taxonomy = await context.Taxonomies.FindAsync(Taxonomyid);

            if (taxonomy == null)
            {
                return NotFound();
            }

            return View(taxonomy);
        }

        // POST /admin/taxonomy/edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Taxonomyid, Taxonomy taxonomy)
        {
            if (ModelState.IsValid)
            {
                var taxonomyCheck = await context.Taxonomies.FirstOrDefaultAsync(x => x.TaxonomyName == taxonomy.TaxonomyName);
                if (taxonomyCheck != null)
                {
                    ModelState.AddModelError("", "The taxonomy already exists.");
                    return View(taxonomy);
                }

                context.Update(taxonomy);
                await context.SaveChangesAsync();

                TempData["Success"] = "The taxonomy has been edited!";

                return RedirectToAction("Edit", new { Taxonomyid });
            }

            return View(taxonomy);
        }

        // GET /admin/taxonomy/delete/id
        public async Task<IActionResult> Delete(int TaxonomyId)
        {
            Taxonomy taxonomy = await context.Taxonomies.FindAsync(TaxonomyId);

            if (taxonomy == null)
            {
                TempData["Error"] = "The taxonomy does not exist!";
            }
            else
            {
                context.Taxonomies.Remove(taxonomy);
                await context.SaveChangesAsync();

                TempData["Success"] = "The taxonomy has been deleted!";
            }

            return RedirectToAction("Index");
        }
    }
}

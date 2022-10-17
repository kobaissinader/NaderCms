using NaderCms.Infrastructure;
using NaderCms.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NaderCms.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin,editor")]
    [Area("Admin")]

    public class TermController : Controller

    {
        private readonly NaderCmsContext context;

        public TermController(NaderCmsContext context)
        {
            this.context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Term> terms = await context.Terms.Include(x => x.Taxonomy).ToListAsync();
            
            return View(terms);

        }

        // GET /admin/term/create
        public IActionResult Create()
        {
            ViewBag.taxonomies = new SelectList(context.Taxonomies.OrderBy(x => x.TaxonomyName), "TaxonomyId", "TaxonomyName");

           return View();
        }
        // POST /admin/term/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Term term)
        {
            ViewBag.taxonomies = new SelectList(context.Taxonomies.OrderBy(x => x.TaxonomyName), "TaxonomyId", "TaxonomyName");

            var termCheck = await context.Terms.FirstOrDefaultAsync(x => x.TermName == term.TermName);
            if (termCheck != null)
                ModelState.AddModelError("", "The term already exists.");

            if (ModelState.IsValid)
            {

                context.Add(term);
                await context.SaveChangesAsync();

                TempData["Success"] = "The term has been added!";

                return RedirectToAction("Index");
            }

            return View(term);
        }

        // GET /admin/term/edit/id

        public async Task<IActionResult> Edit(int Termid)
        {
            Term term = await context.Terms.FindAsync(Termid);
            ViewBag.taxonomies = new SelectList(context.Taxonomies.OrderBy(x => x.TaxonomyName), "TaxonomyId", "TaxonomyName", term.Taxonomy);

            if (term == null)
            {
                return NotFound();
            }

            return View(term);
        }

        // POST /admin/term/edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( Term term)
        {
            ViewBag.taxonomies = new SelectList(context.Taxonomies.OrderBy(x => x.TaxonomyName), "TaxonomyId", "TaxonomyName");

            if (ModelState.IsValid)
            {
                var termCheck = await context.Terms.FirstOrDefaultAsync(x => x.TermName == term.TermName);
                if (termCheck != null)
                {
                    ModelState.AddModelError("", "The term already exists.");
                    return View(term);
                }

                context.Update(term);
                await context.SaveChangesAsync();

                TempData["Success"] = "The term has been edited!";

                return RedirectToAction("Index");
            }

            return View(term);
        }

        // GET /admin/term/delete/id
        public async Task<IActionResult> Delete(int TermId)
        {
            Term term = await context.Terms.FindAsync(TermId);

            if (term == null)
            {
                TempData["Error"] = "The term does not exist!";
            }
            else
            {
                context.Terms.Remove(term);
                await context.SaveChangesAsync();

                TempData["Success"] = "The term has been deleted!";
            }

            return RedirectToAction("Index");
        }
    }
}

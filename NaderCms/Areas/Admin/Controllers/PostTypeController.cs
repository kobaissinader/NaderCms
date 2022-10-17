using NaderCms.Infrastructure;
using NaderCms.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace NaderCms.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin,editor")]
    [Area("Admin")]
    public class PostTypeController : Controller
    {
        private readonly NaderCmsContext context;

        public PostTypeController(NaderCmsContext context)
        {
            this.context = context;
        }
        public async Task<ViewResult> Index()
        {
            List<PostType> postTypes = await context.PostTypes.ToListAsync();

            return View(postTypes);
        }

        // GET /admin/PostType/create
        public async Task<IActionResult> Create()
        {
            ViewBag.taxonomies = await context.Taxonomies.OrderBy(x => x.TaxonomyName).ToListAsync();
             

           return View();
        }

        // POST /admin/PostType/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostType postType, List<int> taxonomy)
        {
            ViewBag.taxonomies = await context.Taxonomies.OrderBy(x => x.TaxonomyName).ToListAsync();
            ViewBag.taxonomiesChecked = await context.PostTypeTaxonomies.Where(x => x.PostTypeId == postType.PostTypeId).ToListAsync();

            if (!ModelState.IsValid)
                return View(postType);

            if (taxonomy == null)
            {
                ModelState.AddModelError("", "The taxonomy list is empty");
                return View(postType);
            }

            context.Add(postType);
            await context.SaveChangesAsync();
            
            foreach( var i in taxonomy)
            {
                PostTypeTaxonomy postTypeTaxonomy = new PostTypeTaxonomy()
                {
                    PostTypeId = postType.PostTypeId,
                    TaxanomyId = i

                };
                context.Add(postTypeTaxonomy);
            }
            await context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET /admin/postType/edit/id

        public async Task<IActionResult> Edit(int PostTypeId)
        {
            PostType postType = await context.PostTypes.FindAsync(PostTypeId);
            ViewBag.taxonomies = await context.Taxonomies.OrderBy(x => x.TaxonomyName).ToListAsync();
            ViewBag.taxonomiesChecked = await context.PostTypeTaxonomies.Where(x => x.PostTypeId == PostTypeId).ToListAsync();
            if (postType == null)
            {
                return NotFound();
            }

            return View(postType);
        }

        // POST /admin/postType/edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( PostType postType, List<int> taxonomy)
        {
            ViewBag.taxonomies = await context.Taxonomies.OrderBy(x => x.TaxonomyName).ToListAsync();
            ViewBag.taxonomiesChecked = await context.PostTypeTaxonomies.Where(x => x.PostTypeId == postType.PostTypeId).ToListAsync();
           
            if (!ModelState.IsValid)
                return View(postType);

            if (taxonomy.Count() == 0)
            {
                ModelState.AddModelError("", "The taxonomy list is empty");
                return View(postType);
            }

            context.Update(postType);
            await context.SaveChangesAsync();
            List<PostTypeTaxonomy> postTypeTaxonomies = await context.PostTypeTaxonomies.Where(x => x.PostTypeId == postType.PostTypeId).ToListAsync();

            foreach ( var x in postTypeTaxonomies)
            {
                context.PostTypeTaxonomies.Remove(x);
            }

            await context.SaveChangesAsync();

            foreach (var i in taxonomy)
            {
                PostTypeTaxonomy postTypeTaxonomy = new PostTypeTaxonomy()
                {
                    PostTypeId = postType.PostTypeId,
                    TaxanomyId = i

                };
                context.Add(postTypeTaxonomy);
            }
            await context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET /admin/postType/delete/id
        public async Task<IActionResult> Delete(int PostTypeId)
        {
            PostType postType = await context.PostTypes.FindAsync(PostTypeId);

            if (postType == null)
            {
                TempData["Error"] = "The postType does not exist!";
            }
            else
            {
                context.PostTypes.Remove(postType);
                await context.SaveChangesAsync();

                TempData["Success"] = "The postType has been deleted!";
            }

            return RedirectToAction("Index");
        }
    }
}

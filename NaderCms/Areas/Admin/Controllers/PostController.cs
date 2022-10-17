using NaderCms.Infrastructure;
using NaderCms.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace NaderCms.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin,editor")]
    [Area("Admin")]
    public class PostController : Controller
    {
        private readonly NaderCmsContext context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public PostController(NaderCmsContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;

        }
        public  async Task<IActionResult> Index()
        {
            List<Post> posts = await context.Posts.Include(x => x.PostType).ToListAsync();

            return View(posts);
        }

        
        // GET /admin/Post/create
        public async Task<IActionResult> Create(int? postTypeId = null )
        {

            ViewBag.postTypeId = postTypeId;

            if (!postTypeId.HasValue)
            {
                ViewBag.postType = await context.PostTypes.ToListAsync();
                return View();
            }
            else
            {
                ViewBag.PostType = await context.PostTypes.FindAsync(postTypeId);

                var taxonomies = await context.PostTypeTaxonomies.Where(x => x.PostTypeId == postTypeId).ToListAsync();

                List<int> TaxonomiesId = new List<int>();
                
                foreach ( var i in taxonomies)
                {
                    TaxonomiesId.Add(i.TaxanomyId);
                }

                var terms = await context.Terms.Where(x => TaxonomiesId.Contains(x.TaxonomyId)).ToListAsync();

                ViewBag.terms = terms;

                return View();

            }
        }

        //POST /admin/Post/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post, List<int> termIds)
        {
            ViewBag.postTypes = await context.PostTypes.OrderBy(x => x.PostTypeTitle).ToListAsync();
            //ViewBag.postTypeId = post.PostTypeId;
            //ViewBag.PostType = await context.PostTypes.FindAsync(post.PostTypeId);
            post.PostCreatoinDate = DateTime.Now;
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Create",new { postTypeId = post.PostTypeId } );
            }
            if (termIds == null || termIds.Count == 0)
            {
                ModelState.AddModelError("", "At least one term must be chosen!");
                return RedirectToAction("Create", new { postTypeId = post.PostTypeId });
            }
            context.Add(post);
            await context.SaveChangesAsync();

            string imageName = "noimage.png";
            if (post.ImageUpload != null)
            {
                string uploadsDir = Path.Combine(webHostEnvironment.WebRootPath, "media/images");
                imageName = Guid.NewGuid().ToString() + " " + post.ImageUpload.FileName;
                string filePath = Path.Combine(uploadsDir, imageName);
                FileStream fs = new FileStream(filePath, FileMode.Create);
                await post.ImageUpload.CopyToAsync(fs);
                fs.Close();
                post.Image = imageName;
            }

            foreach (var i in termIds)
            {
                PostTerm postTerm = new PostTerm()
                {
                    PostId = post.PostId,
                    TermId = i

                };
                context.Add(postTerm);
            }
            await context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET /admin/post/edit/id

        public async Task<IActionResult> Edit(int PostId)
        {
            var post = await context.Posts.FindAsync(PostId);
            ViewBag.postType = await context.PostTypes.FindAsync(post.PostTypeId);

            var taxonomies = await context.PostTypeTaxonomies.Where(x => x.PostTypeId == post.PostTypeId).ToListAsync();

            List<int> TaxonomiesId = new List<int>();

            foreach (var i in taxonomies)
            {
                TaxonomiesId.Add(i.TaxanomyId);
            }

            var terms = await context.Terms.Where(x => TaxonomiesId.Contains(x.TaxonomyId)).ToListAsync();

            ViewBag.terms = terms;

            return View(post);
        }

        // post /admin/post/edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Post post, List<int> termIds)
        {
            if (!ModelState.IsValid)
                return View(post);

            if (termIds.Count() == 0)
            {
                ModelState.AddModelError("", "The taxonomy list is empty");
                return View(post);
            }

            if (post.ImageUpload != null)
            {
            
                string uploadsDir = Path.Combine(webHostEnvironment.WebRootPath, "media/images");

                if (!string.Equals(post.Image, "noimage.png"))
                {
                    string oldImagePath = Path.Combine(uploadsDir, post.Image);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                string imageName = Guid.NewGuid().ToString() + " " + post.ImageUpload.FileName;
                string filePath = Path.Combine(uploadsDir, imageName);
                FileStream fs = new FileStream(filePath, FileMode.Create);
                await post.ImageUpload.CopyToAsync(fs);
                fs.Close();
                post.Image = imageName;
            }

            context.Update(post);
            await context.SaveChangesAsync();

            List<PostTerm> postTerms = await context.PostTerms.Where(x => x.PostId == post.PostId).ToListAsync();

            foreach (var x in postTerms)
            {
                context.PostTerms.Remove(x);
            }

            await context.SaveChangesAsync();

            foreach (var i in termIds)
            {
                PostTerm postTerm = new PostTerm()
                {
                    PostId = post.PostId,
                    TermId = i

                };
                context.Add(postTerm);
            }
            await context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int PostId)
        {
            Post post = await context.Posts.FindAsync(PostId);
            context.Remove(post);
            await context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}

using NaderCms.Infrastructure;
using NaderCms.Models;
using UserWebsite.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace UserWebsite.Controllers
{
    public class UserHomeController : Controller
    {
        private readonly NaderCmsContext context;

        public UserHomeController(NaderCmsContext context)
        {
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel indexViewModel = new IndexViewModel();

            indexViewModel.lastFivePosts = await context.Posts.OrderByDescending(x => x.PostId).Where(x => x.PostTypeId != 6).Take(5).ToListAsync();

            var lastThreePostTypes = await context.PostTypes.OrderByDescending(x => x.PostTypeId).Take(3).ToListAsync();

            indexViewModel.indexTerms = await context.Terms.OrderByDescending(x => x.TermId).ToListAsync();


            //First PostType 
            ViewBag.NewestPostType = await context.PostTypes.FindAsync(lastThreePostTypes[0].PostTypeId);
            ViewBag.NewestPostTypePosts = await context.Posts.Where(x => x.PostTypeId == lastThreePostTypes[0].PostTypeId).Take(5).ToListAsync();

            ////Second PostType 
            ViewBag.SecondNewestPostType = await context.PostTypes.FindAsync(lastThreePostTypes[1].PostTypeId);
            ViewBag.SecondNewestPostTypePosts = await context.Posts.Where(x => x.PostTypeId == lastThreePostTypes[1].PostTypeId).Take(5).ToListAsync();

            ////Third PostType 
            ViewBag.ThirdNewestPostType = await context.PostTypes.FindAsync(lastThreePostTypes[2].PostTypeId);
            ViewBag.ThirdNewestPostTypePosts = await context.Posts.Where(x => x.PostTypeId == lastThreePostTypes[2].PostTypeId).Take(4).ToListAsync();

            //Taxonomies
            //ViewBag.Taxonomies = await context.Taxonomies.OrderByDescending(x => x.TaxonomyId).Take(6).ToListAsync();

            //Last 3 post for shared layout
            ViewBag.LastThreePostsSharedLayout = await context.Posts.OrderByDescending(x => x.PostId).Take(3).ToListAsync();
            return View(indexViewModel);
        }

        public async Task<IActionResult> TaxonomyIndex(int TaxonomyId)
        {
            ViewBag.taxonomy = await context.Taxonomies.FindAsync(TaxonomyId);
            var terms = await context.Terms.Where(x => x.TaxonomyId == TaxonomyId).ToListAsync();


            //var postType = await context.PostTypeTaxonomies.Where(x => x.TaxanomyId == TaxonomyId).ToListAsync();

            List<PostTerm> postTerms = new List<PostTerm>();

            foreach (var i in terms)
            {
                postTerms.AddRange(await context.PostTerms.Where(x => x.TermId == i.TermId).ToListAsync());
            };

            List<int> postsId = new List<int>();

            foreach (var i in postTerms)
            {
                postsId.Add(i.PostId);
            }

            var postsIdWithoutDup = postsId.Distinct().ToList();

            ViewBag.TaxonomyPosts = await context.Posts.Where(x => postsIdWithoutDup.Contains(x.PostId)).ToListAsync();

           
            return View();
        }

        public async Task<IActionResult> TermIndex(int TermId)
        {
            var term = await context.Terms.FindAsync(TermId);

            ViewBag.term = term;

            

            List< PostTerm> postTerms = new List<PostTerm>();

            
             postTerms = await context.PostTerms.Where(x => x.TermId == term.TermId).ToListAsync();


            List<int> postsId = new List<int>();

            foreach (var i in postTerms)
            {
                postsId.Add(i.PostId);
            }

            //var postsIdWithoutDup = postsId.Distinct().ToList();

            ViewBag.TermPosts = await context.Posts.Where(x => postsId.Contains(x.PostId)).ToListAsync();

           
            return View();
        }

        public async Task<IActionResult> SinglePost(int postId)
        {
            var post = await context.Posts.FindAsync(postId);

            ViewBag.terms = await context.Terms.OrderByDescending(x => x.TermId).ToListAsync();
            ViewBag.post = post;
            return View();
        }


        public IActionResult Contact()
        {
            return View();
        }
            public IActionResult Privacy()
        {
            return View();
        }

        
    }
}

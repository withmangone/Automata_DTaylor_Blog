using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using Automata_DTaylor_Blog.Helpers;
using Automata_DTaylor_Blog.Models;
using PagedList;
using PagedList.Mvc;

namespace Automata_DTaylor_Blog.Controllers
{
    [RequireHttps]
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BlogPosts
        public ActionResult Index(int? page, string searchStr)
        {
            ViewBag.Search = searchStr;
            var blogList = IndexSearch(searchStr);
            int pageSize = 6; // display 6 blog posts at a time on this page
            int pageNumber = (page ?? 1);
            return View(blogList.ToPagedList(pageNumber, pageSize));
        }
        
        // GET: Index
        public IQueryable<BlogPost> IndexSearch(string searchStr)
        {
            IQueryable<BlogPost> result = null;
            if (searchStr != null)
            {
                result = db.BlogPosts.Where(m => m.Published).AsQueryable();
                result = result.Where(p => p.Title.Contains(searchStr) || p.Body.Contains(searchStr) || p.Comments.Any(c => c.CommentBody.Contains(searchStr) || c.Author.FirstName.Contains(searchStr) || c.Author.LastName.Contains(searchStr) || c.Author.DisplayName.Contains(searchStr) || c.Author.Email.Contains(searchStr)));
                if (result.Any() == false)
                {
                    //System.Windows.Forms.MessageBox.Show("Your search returned no results.");
                    result = db.BlogPosts.Where(m => m.Published).AsQueryable();
                }                   
            }
            else
            {
                result = db.BlogPosts.Where(m => m.Published).AsQueryable();
            }
            return result.OrderByDescending(p => p.Created);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AdminIndex()
        {
            var allBlogPosts = db.BlogPosts.ToList();
            return View(allBlogPosts);
        }

        // GET: BlogPosts/Details/5
        public ActionResult Details(string Slug)
        {
            if (String.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.FirstOrDefault(p => p.Slug == Slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Create
        [Authorize(Roles  = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Body,Abstract,QuoteInit,Quote")] BlogPost blogPost, HttpPostedFileBase image, HttpPostedFileBase thumbImage)
        {
            if (ModelState.IsValid)
            {
                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blogPost.MediaUrl = "/Uploads/" + fileName;
                }

                if (ImageUploadValidator.IsWebFriendlyImage(thumbImage))
                {
                    var fileName = Path.GetFileName(thumbImage.FileName);
                    thumbImage.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blogPost.ThumbUrl = "/Uploads/" + fileName;
                }

                var Slug = StringUtilities.URLFriendly(blogPost.Title);
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title. Cannot be blank.");
                    return View(blogPost);
                }
                if (db.BlogPosts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique.");
                    return View(blogPost);
                }

                blogPost.Slug = Slug;
                blogPost.Created = DateTimeOffset.UtcNow.ToOffset(new TimeSpan(-4, 0, 0));
                db.BlogPosts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Body,Abstract,MediaUrl,Published,Created,Slug,QuoteInit,Quote,ThumbUrl")] BlogPost blogPost, HttpPostedFileBase image, HttpPostedFileBase thumbImage)
        {
            if (ModelState.IsValid)
            {
                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blogPost.MediaUrl = "/Uploads/" + fileName;
                }

                if (ImageUploadValidator.IsWebFriendlyImage(thumbImage))
                {
                    var fileName = Path.GetFileName(thumbImage.FileName);
                    thumbImage.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blogPost.ThumbUrl = "/Uploads/" + fileName;
                }

                var slug = StringUtilities.URLFriendly(blogPost.Title);
                if (slug != blogPost.Slug)
                {
                    if (String.IsNullOrWhiteSpace(slug))
                    {
                        ModelState.AddModelError("Title", "Invalid title. Cannot be blank.");
                        return View(blogPost);
                    }
                    if (db.BlogPosts.Any(p => p.Slug == slug))
                    {
                        ModelState.AddModelError("Title", "The title must be unique.");
                        return View(blogPost);
                    }
                    blogPost.Slug = slug;
                }

                blogPost.Updated = DateTimeOffset.UtcNow.ToOffset(new TimeSpan(-4, 0, 0));
                db.Entry(blogPost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.BlogPosts.Find(id);
            db.BlogPosts.Remove(blogPost);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

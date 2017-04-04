using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Blog.Models;
using Microsoft.AspNet.Identity;
using System.IO;
using PagedList;
using PagedList.Mvc;

namespace Blog.Controllers
{
    [RequireHttps]
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BlogPosts
        public ActionResult Index(int? page)
        {
            ViewBag.ReturnUrl = Request.Url.LocalPath;
            //BlogVM BlogPost = new BlogVM();
            //BlogPost.BlogPost = db.Posts.ToList();
            //if (db.Topics.Count() == 0)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //else
            //{
            //    BlogPost.Topics = db.Topics.ToList();
            //};
            //BlogPost.Comments = db.Comments.ToList();
            //return View(BlogPost);
            int pageSize = 3; // display three blog posts at a time on this page
            int pageNumber = (page ?? 1);
            return View(db.Posts.AsQueryable().OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
        }

        // GET: BlogPosts/Details/5
        public ActionResult Details(string Slug)
        {
            ViewBag.ReturnUrl = Request.Url.AbsolutePath; //Request.Url.AbsolutePath is the URL that is fed to the controller from whatever was clicked it to send it here (a.k.a. the URL of the location the controller is directing you to.) 
            if (Slug == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.FirstOrDefault(b => b.Slug == Slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }
        [Authorize(Roles = "Admin")]
        // GET: BlogPosts/Create
        public ActionResult Create()
        {
            ViewBag.ReturnUrl = Request.Url.LocalPath;
            ViewBag.Topics = new MultiSelectList(db.Topics, "Id", "Name");
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Modified,ModifiedNote,Private,Title,Body,MediaURL,Published")] BlogPost blogPost, HttpPostedFileBase image, List<string> Topics)
        {
            if (ModelState.IsValid)
            {
                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blogPost.MediaURL = "/Uploads/" + fileName;
                }

                var user = db.Users.Find(User.Identity.GetUserId());
                blogPost.Author = user.Id;
                if (Topics != null)
                {
                    var TopicsList = new List<Topics>();
                    foreach (var item in Topics)
                    {
                        int intItem = Convert.ToInt32(item);
                        var TopicNum = db.Topics.Find(intItem);
                        TopicsList.Add(TopicNum);
                    }
                    blogPost.Topics = TopicsList;
                }
                var Slug = StringUtilities.URLFriendly(blogPost.Title);
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(blogPost);
                }
                if (db.Posts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(blogPost);
                }

                blogPost.Slug = Slug;
                blogPost.Created = DateTimeOffset.Now;

                db.Posts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Topics = new MultiSelectList(db.Topics, "Id", "Name");
            return View(blogPost);
        }

        [Authorize(Roles = "Admin")]
        // GET: BlogPosts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            var selectedBlogIds = new List<int>();
            foreach (var item in blogPost.Topics)
            {
                selectedBlogIds.Add(item.Id);
            }

            ViewBag.ReturnUrl = Request.Url.LocalPath;
            ViewBag.Topics = new MultiSelectList(db.Topics, "Id", "Name", selectedBlogIds);
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Created,Modified,ModifiedNote,Private,Title,Body,MediaURL,Published")] BlogPost blogPost, HttpPostedFileBase image, List<string> Topics)
        {
            var blog = db.Posts.Find(blogPost.Id);
            if (ModelState.IsValid)
            {
                //I had to do this because for some reason it quit recognizing blogPost as being in the database.
                blog.Title = blogPost.Title;
                blog.Body = blogPost.Body;
                blog.ModifiedNote = blogPost.ModifiedNote;
                blog.Private = blogPost.Private;
                blog.Published = blogPost.Published;

                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blog.MediaURL = "/Uploads/" + fileName;
                }

                var Slug = StringUtilities.URLFriendly(blogPost.Title);
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(blogPost);
                }
                //if (db.Posts.Any(p => p.Slug == Slug))
                //{
                //    ModelState.AddModelError("Title", "The title must be unique");
                //    return View(blogPost);
                //}

                blog.Topics.Clear();
                //foreach (var item in blog.Topics)
                //{
                //    blog.Topics.Remove(item);

                //    //db.Entry(blog).Property("Topics").IsModified = true;
                //} This way is wrong. Use .Clear() instead.
                //Once all the topics have been cleared out, you should add the new ones in.
                if (Topics != null)
                {
                    foreach (var item in Topics)
                    {
                        int intItem = Convert.ToInt32(item);
                        var TopicNum = db.Topics.Find(intItem);
                        blog.Topics.Add(TopicNum);
                    }
                }
                blog.Slug = Slug;
                blog.Modified = DateTimeOffset.Now;
                db.Entry(blog).State = EntityState.Modified; //EntityState.Modified says that if anything has changed, you updated all values in the table for this case.
                db.SaveChanges();
                return RedirectToAction("Details", "BlogPosts", new { Slug = Slug });
            }
            return View(blog);
        }

        [Authorize(Roles = "Admin")]
        // GET: BlogPosts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.Find(id);
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
            BlogPost blogPost = db.Posts.Find(id);
            db.Posts.Remove(blogPost);
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

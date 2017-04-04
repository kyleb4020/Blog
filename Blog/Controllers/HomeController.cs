using Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Mail;

namespace Blog.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            //var max = db.Posts.OrderByDescending(p => p.Id).FirstOrDefault();
            //BlogPost blogPost = max;
            //if (blogPost == null)
            //{
            //    return HttpNotFound();
            //}
            ViewBag.ReturnUrl = Request.Url.LocalPath;
            return View(db.Posts.ToList());
        }

        //public ActionResult Topics()
        //{
        //    ViewBag.Message = "Feel free to browse through the blog posts by topic.";

        //    return View();
        //}

        public ActionResult About()
        {
            ViewBag.Message = "Here is my story. Short Version: I took the scenic route.";
            ViewBag.ReturnUrl = Request.Url.LocalPath;

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.ReturnUrl = Request.Url.LocalPath;
            ViewBag.StatusMessage = "";
            ViewBag.Message = "If you wish to contact me for some reason...";
            EmailModel model = new EmailModel();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = "<p>Email From: <bold>{0}</bold>({1})</p><p> Message:</p><p>{2}</p>";
                    var from = "MyBlog<kylecoder@gmail.com>";
                    model.Body = "This is a message from your blog site. The name and the email of the contacting person is above.";
                    var email = new MailMessage(from,
                         ConfigurationManager.AppSettings["emailto"])
                    {
                        Subject = "Blog Contact Email",
                        Body = String.Format(body, model.FromName, model.FromEmail, model.Body),
                        //Body = "test",
                        IsBodyHtml = true
                    };
                    var svc = new PersonalEmail();
                    await svc.SendAsync(email);


                    ViewBag.StatusMessage = "Success! Thanks for reaching out to me!";
                    //return RedirectToAction("Sent");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
            }
            return View(model);
        }

        //public ActionResult Search(int? page)
        //{
        //    var Model = db.Posts.AsQueryable();
        //    int pageSize = 5; // display three blog posts at a time on this page
        //    int pageNumber = (page ?? 1);
        //    return View(Model.OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [HttpGet]
        public ActionResult Search(int? page, string search)
        {
            ViewBag.ReturnUrl = Request.Url.LocalPath;
            var blogResults = db.Posts.AsQueryable(); //Most efficient method
            ViewBag.search = search;
            if (!String.IsNullOrEmpty(search))
            {
                blogResults = db.Posts.Where(p => p.Body.Contains(search) || p.Title.Contains(search)
                                || p.Slug.Contains(search) || p.Comments.Any(c => c.Body.Contains(search)
                                || c.Author.DisplayName.Contains(search) || c.Author.FirstName.Contains(search)
                                || c.Author.LastName.Contains(search)) || p.Topics.Any(t=>t.Name.Contains(search)));

                //Slightly less efficient method
                //blogResults = db.Posts.Where(p => p.Title.Contains(search))
                //    .Union(db.Posts.Where(p => p.Slug.Contains(search)))
                //    .Union(db.Posts.Where(p => p.Body.Contains(search)))
                //    .Union(db.Posts.Where(p => p.Comments.Any(c => c.Body.Contains(search))))
                //    .Union(db.Posts.Where(p => p.Comments.Any(c => c.Author.DisplayName.Contains(search))))
                //    .Union(db.Posts.Where(p => p.Comments.Any(c => c.Author.LastName.Contains(search))))
                //    .Union(db.Posts.Where(p => p.Comments.Any(c => c.Author.FirstName.Contains(search))))
                //    .Union(db.Posts.Where(p => p.Comments.Any(c => c.Author.Email.Contains(search))))
                //    /*.Union(db.Posts.Where(p => p.Topics.Any(t => t.Name.Contains(search))))*/.AsQueryable();

                //Very inefficient method
                //var title = db.Posts.Where(p=>p.Title.Contains(search));
                //var blogResults = title.Union(db.Posts.Where(p => p.Slug.Contains(search)))
                //    .Union(db.Posts.Where(p => p.Body.Contains(search)))
                //var comment = db.Comments.Where(c => c.Body.Contains(search)).Union(db.Comments.Where(c => c.Author.;
                //foreach (var c in comment)
                //{
                //    blogResults.Union(db.Posts.Where(p => p.Id.Equals(c.BlogPostId)));
                //}
                //var topic = db.Topics.Where(c => c.Name.Contains(search));
                //foreach (var t in topic)
                //{
                //    foreach (var b in t.Blogs)
                //    {
                //        blogResults.Union(db.Posts.Where(p => p.Id.Equals(b.Id)));
                //    }
                //}

                //Session["search"] = blogResults;
                int pageSize = 3; // display three blog posts at a time on this page
                int pageNumber = (page ?? 1);
                //ViewBag.search = blogResults;
                return View(blogResults.OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
                //List<BlogPost> resultsList = blogResults.ToList();
                //return View(resultsList);
            }
            //else if (page != null && String.IsNullOrEmpty(search))
            //{
            //    blogResults = Session["search"] as IQueryable<BlogPost>;

            //    int pageSize = 3; // display three blog posts at a time on this page
            //    int pageNumber = (page ?? 1);
            //    //ViewBag.search = blogResults;
            //    return View(blogResults.OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
            //}
            else
            {
                //Session["search"] = null;
                int pageSize = 3; // display three blog posts at a time on this page
                int pageNumber = (page ?? 1);
                return View(blogResults.OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
                //return RedirectToLocal(Request.Url.LocalPath);
            }

        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
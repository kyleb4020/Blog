using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Models
{
    public class Comments
    {
        public int Id { get; set; }
        public string AuthorId { get; set; }
        public int BlogPostId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Modified { get; set; }
        public string ModifiedNote { get; set; }
        [AllowHtml]
        public string Body { get; set; }
        public string ProfilePicURL { get; set; }
        public bool Private { get; set; }
        public virtual ApplicationUser Author { get; set; }
        public virtual BlogPost BlogPost {get; set;}
    }
}
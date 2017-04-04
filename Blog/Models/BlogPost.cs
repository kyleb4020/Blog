using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Models
{
    public class BlogPost
    {
        public BlogPost() //This is a constructor
        {
            this.Comments = new HashSet<Comments>();
            this.Topics = new HashSet<Topics>();
        }

        public int Id { get; set; }
        public string Author { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Modified { get; set; }
        public string ModifiedNote { get; set; }
        public bool Private { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        [AllowHtml]
        public string Body { get; set; }
        public string MediaURL { get; set; } //What if I want multiple?
        public virtual ICollection<Topics> Topics { get; set; }
        public bool Published { get; set; }
        public virtual ICollection<Comments> Comments { get; set; }
    }
}
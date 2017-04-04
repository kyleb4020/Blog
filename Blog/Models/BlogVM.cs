using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class BlogVM
    {
        public List<BlogPost> BlogPost { get; set; }
        public List<Topics> Topics { get; set; }
        public List<Comments> Comments { get; set; }
    }
}
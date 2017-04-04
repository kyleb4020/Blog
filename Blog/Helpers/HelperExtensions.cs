using Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Blog.Helpers
{
    public static class HelperExtensions
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        //I made this so I could add DisplayName as a method onto an IIdentity.
        //This is useful for me because it displays their DisplayName in the _LoginPartial
        public static string DisplayName(this IIdentity user)
        {
            var disName = db.Users.FirstOrDefault(u => u.UserName == user.Name);
            if (disName.DisplayName != null)
            {
                return disName.DisplayName;
            }
            else
            {
                return "Manage Account";
            }
        }
    }
}
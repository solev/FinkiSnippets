using FinkiSnippets.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Entity
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(CodeDatabase db) : base(new UserStore<ApplicationUser>(db))
        {
            UserValidator = new UserValidator<ApplicationUser>(this) { AllowOnlyAlphanumericUserNames = false }; 
        }
    }
}
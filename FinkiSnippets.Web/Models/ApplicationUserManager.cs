using Data_Access;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Models
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {

        public ApplicationUserManager()
            : base(new UserStore<ApplicationUser>(new CodeDatabase()))
        {
            UserValidator = new UserValidator<ApplicationUser>(this) { AllowOnlyAlphanumericUserNames = false }; 
        }
    }
}
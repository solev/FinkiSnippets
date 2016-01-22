using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Data_Access;
using App.ViewModels;
using App.Models;



namespace App.Controllers
{
           
    public class UserController : Controller
    {
        ApplicationUserManager userManager = new ApplicationUserManager();
        CodeDatabase db = new CodeDatabase();
        
        public ActionResult Login()
        {

            if(User.Identity.IsAuthenticated)
            {
                if(User.IsInRole("Admin"))
                {
                    return RedirectToAction("Users", "Admin", new {id=1 });
                }
                return RedirectToAction("Start", "Code");            
            }
                
            return View();
        }



        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {            
            if(ModelState.IsValid)
            {
                var user = userManager.Find(model.Username, model.Password);            
                if(user != null)
                {
                    var authManager = HttpContext.GetOwinContext().Authentication;
                    authManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                    var identity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                    authManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, identity);

                    return RedirectToAction("Login");
                }                
            }
            ViewBag.error = "Корисничкото име или лозинката ви е погрешна";
            return View(model);
        }

        
        public ActionResult Logout()
        {
            var authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignOut();

            return RedirectToAction("Login");
        }
    }
}
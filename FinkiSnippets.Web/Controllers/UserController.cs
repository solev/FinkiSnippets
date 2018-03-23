using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using App.ViewModels;
using App.Models;
using Entity;



namespace App.Controllers
{           
    public class UserController : Controller
    {
        ApplicationUserManager _userManager;
        
        public UserController(ApplicationUserManager userManager)
        {
            _userManager = userManager;
        }
        
        public ActionResult Login()
        {

            if(User.Identity.IsAuthenticated)
            {
                if(User.IsInRole("Admin"))
                {
                    return RedirectToAction("Users", "Admin");
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
                var user = _userManager.Find(model.Username, model.Password);            
                if(user != null)
                {
                    var authManager = HttpContext.GetOwinContext().Authentication;
                    authManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                    var identity = _userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                    authManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, identity);

                    return RedirectToAction("Login");
                }                
            }

            ViewBag.error = "Корисничкото име или лозинката ви е погрешна";            

            return View(model);
        }

        [NonAction]
        public string HashPassword(string password)
        {
            var hashed = _userManager.PasswordHasher.HashPassword(password);
            return hashed;
        }
        
        public ActionResult Logout()
        {
            var authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignOut();

            return RedirectToAction("Login");
        }
    }
}
using App.Models;
using App.ViewModels;
using Entity;
using FinkiSnippets.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace App.Controllers
{
    [Authorize]
    public class CodeController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly IEventService _eventService;
        private readonly ISnippetService _snippetService;
        public CodeController(IEventService eventService, ISnippetService snippetService,ApplicationUserManager userManager)
        {
            _eventService = eventService;
            _snippetService = snippetService;
            _userManager = userManager;
        }

        public static DateTime getMyTime()
        {
            return DateTime.Now.AddHours(1);
        }

        public ActionResult Start()
        {
            Event ev = _eventService.GetCurrentEvent();

            if(ev!=null)
                return View(ev);

            return View();
        }

        //id == orderNumber
        public ActionResult Game()
        {
            DateTime t = DateTime.Now.AddHours(1);

            var ev = db.Events.FirstOrDefault(x => x.Start < t && x.End > t);
            if (ev == null)
                return RedirectToAction("Start");

            AnswerLog last = null;

            try
            {
                last = db.Answers.Where(x => x.User.UserName == User.Identity.Name && x.Event.ID == ev.ID && x.answered && x.snippet.Group.ID == ev.Group.ID).OrderByDescending(x => x.snippet.OrderNumber).Take(1).Single();
            }
            catch
            {
                last = null;
            }

            int id = 1;
            if (last != null)
                id = last.snippet.OrderNumber + 1;

            Snippet snippet;
            int lastOrderNumber = db.Snippets.Where(x => x.Group.ID == ev.Group.ID).OrderByDescending(x => x.ID).Take(1).Single().OrderNumber;
            if (id > lastOrderNumber)
            {
                return RedirectToAction("Result", new { status = "YzK12QQu" });
            }

            snippet = db.Snippets.FirstOrDefault(x => x.OrderNumber == id && x.Group.ID == ev.Group.ID);

            var check = db.Answers.FirstOrDefault(x => x.User.UserName == User.Identity.Name && x.snippet.ID == snippet.ID && x.Event.ID == ev.ID);

            if (check == null)
            {
                var user = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
                db.Answers.Add(new AnswerLog { DateCreated = getMyTime(), User = user, snippet = snippet, Event = ev });
                db.SaveChanges();
            }
            ViewBag.lastOrderNumber = lastOrderNumber;
            return View(snippet);
        }
        
        [HttpPost]
        public ActionResult NextSnippet(int id, string answer)
        {
            DateTime t = DateTime.Now.AddHours(1);

            string userID = User.Identity.GetUserId();
            int snippetID = id;
            bool res = _snippetService.SubmitAnswer(userID, snippetID, answer);

            if(!res)
            {

            }

            return RedirectToAction("Game");
        }

        public ActionResult Result(string status)
        {
            if (status != null && status != "YzK12QQu")
                return RedirectToAction("Game");
            return View();
        }
              

    }
}
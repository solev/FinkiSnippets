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
using Utilities;

namespace App.Controllers
{
    [Authorize]
    public class CodeController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly IEventService _eventService;
        private readonly ISnippetService _snippetService;
        public CodeController(IEventService eventService, ISnippetService snippetService, ApplicationUserManager userManager)
        {
            _eventService = eventService;
            _snippetService = snippetService;
            _userManager = userManager;
        }

        
        public ActionResult Start()
        {
            List<Event> NextEvents = _eventService.GetNextEvents();
            List<Event> ActiveEvents = _eventService.GetActiveEvents();

            StartViewModel model = new StartViewModel { ActiveEvents = ActiveEvents, NextEvents = NextEvents };
            return View(model);
        }

        //id == orderNumber
        public ActionResult Game()
        {
            //DateTime t = DateTime.Now.AddHours(1);
            
            //// I DONT NEED ALL EVENT DATA
            //var ev = _eventService.GetCurrentEvent();

            ////no event at current time
            //if (ev == null)
            //    return RedirectToAction("Start");


            //int lastAnsweredOrderNumber = _snippetService.GetLastAnsweredSnippetOrderNumber(User.Identity.GetUserId(), ev.ID, 1);

            //int orderNumber = lastAnsweredOrderNumber + 1;

            //Snippet snippet;

            //int lastOrderNumber = _snippetService.GetLastSnippetOrderNumber(ev.Group.ID);

            //if (orderNumber > lastOrderNumber)
            //{

            //    return RedirectToAction("Result", new { status = "YzK12QQu" });
            //}

            //snippet = _snippetService.GetSnippetWithOrderNumber(orderNumber, ev.Group.ID);

            //bool check = _snippetService.CheckIfFirstSnippetAccess(User.Identity.GetUserId(),snippet.ID,ev.ID);

            //if (!check)
            //{
            //    ApplicationUser user = _userManager.FindById(User.Identity.GetUserId());
            //    bool res = _snippetService.CreateInitialAnswer(new AnswerLog { DateCreated = DateHelper.GetCurrentTime(), User = user, snippet = snippet, Event = ev });                
            //}

            //ViewBag.lastOrderNumber = lastOrderNumber;
            return View();
        }

        [HttpPost]
        public ActionResult NextSnippet(int id, string answer)
        {
            DateTime t = DateTime.Now.AddHours(1);

            string userID = User.Identity.GetUserId();
            int snippetID = id;
            bool res = _snippetService.SubmitAnswer(userID, snippetID, answer);

            if (!res)
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
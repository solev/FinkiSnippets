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
        private readonly IUserService _userService;
        private readonly IEventService _eventService;
        private readonly ISnippetService _snippetService;

        public CodeController(IEventService eventService, ISnippetService snippetService, ApplicationUserManager userManager, IUserService userService)
        {
            _eventService = eventService;
            _snippetService = snippetService;
            _userService = userService;
            _userManager = userManager;
        }

        
        public ActionResult Start()
        {
            List<Event> NextEvents = _eventService.GetNextEvents();
            List<Event> ActiveEvents = _eventService.GetActiveEvents();

            StartViewModel model = new StartViewModel { ActiveEvents = ActiveEvents, NextEvents = NextEvents };
            return View(model);
        }

        public ActionResult Lobby(int id)
        {
            Event ev = _eventService.GetEventById(id);

            return View(ev);
        }

        //id == orderNumber
        public ActionResult Game(int id)
        {
            string userID = User.Identity.GetUserId();

            Event userActiveEvent = _userService.UserActiveEvent(userID);
            var validateEvent = _eventService.GetEventById(id);

            //Event doesnt exist
            if (validateEvent == null)
            {                
                return RedirectToAction("Start");
            }

            //Has no active events start a new one
            if (userActiveEvent == null)
            {   
                _userService.BeginEvent(User.Identity.GetUserId(), validateEvent.ID);
                userActiveEvent = validateEvent;

                //return view with first snippet
            }
            
            //Has active event but wants to start another one
            if(userActiveEvent.ID != validateEvent.ID)
            {
                // should redirect to lobby of the event
                return RedirectToAction("Start");
            }            



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
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
                       
            var userActiveEvent = _userService.UserActiveEvent(userID);
            var validateEvent = _eventService.GetEventById(id);

            //Event doesnt exist , its finished or it is not started yet
            if (validateEvent == null || validateEvent.End < DateHelper.GetCurrentTime() || validateEvent.Start > DateHelper.GetCurrentTime())
            {                
                return RedirectToAction("Start");
            }

            //Has no active events start a new one
            if (userActiveEvent == null)
            {   
                
                EventSnippets firstSnippet = _userService.BeginEvent(User.Identity.GetUserId(), validateEvent.ID);

                //Already participated and finished
                if(firstSnippet == null)
                    return RedirectToAction("Start");

                _snippetService.CreateInitialAnswer(userID, firstSnippet.EventID,firstSnippet.SnippetID);
                userActiveEvent = _userService.UserActiveEvent(userID);
                ViewBag.lastOrderNumber = userActiveEvent.OrderNumber;
                return View(firstSnippet);
                //return view with first snippet
            }
            
            //Has active event but wants to start another one
            if(userActiveEvent.EventID != validateEvent.ID)
            {
                // should redirect to lobby of the event
                return RedirectToAction("Lobby", new { id = userActiveEvent.EventID });
            }

            int lastAnsweredOrderNumber = _snippetService.GetLastAnsweredSnippetOrderNumber(userID, userActiveEvent.EventID);
            if(++lastAnsweredOrderNumber > userActiveEvent.OrderNumber)
            {
                _eventService.FinishEventForUser(userActiveEvent.EventID, userID);
                TempData["EventFinished"] = true;
                return RedirectToAction("Result");
            }

            
            var model = _snippetService.GetSnippetWithOrderNumber(lastAnsweredOrderNumber, userActiveEvent.EventID);
            _snippetService.CreateInitialAnswer(userID, userActiveEvent.EventID, model.SnippetID);
            ViewBag.lastOrderNumber = userActiveEvent.OrderNumber;

            return View(model);
        }

        [HttpPost]
        public ActionResult NextSnippet(int SnippetID, int EventID ,string answer)
        {
            
            string userID = User.Identity.GetUserId();            
            bool res = _snippetService.SubmitAnswer(userID, EventID, SnippetID, answer);
                      
            return RedirectToAction("Game", new { id = EventID});
        }

        public ActionResult Result()
        {

            if (TempData["EventFinished"] == null)
                return RedirectToAction("Game", new { id = 0 });

            return View();
        }


    }
}
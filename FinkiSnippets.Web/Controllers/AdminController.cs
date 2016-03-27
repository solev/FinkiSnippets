using App.Models;
using App.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
//using Excel;
using ExportToExcel;
using DocumentFormat.OpenXml.Packaging;
using System.Data;
using System.Data.Entity;
using Entity;
using FinkiSnippets.Data;
using FinkiSnippets.Service;
using FinkiSnippets.Service.Dto;
using FinkiSnippets.Service.Groups;
using System.Threading;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Text;

namespace App.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        ApplicationUserManager _userManager;
        private readonly IExportService _exportService;
        private readonly IUserService _userService;
        private readonly ISnippetService _snippetService;
        private readonly IEventService _eventService;
        private readonly IGroupService _groupService;

        public AdminController(ApplicationUserManager userManager, IExportService exportService, IUserService userService, ISnippetService snippetService, IEventService eventService, IGroupService groupService)
        {
            _userManager = userManager;
            _exportService = exportService;
            _userService = userService;
            _snippetService = snippetService;
            _eventService = eventService;
            _groupService = groupService;
        }
                
        public void ExportResults(int id)
            {
            //Create table for results
            var result = _exportService.ExportResultsForEvent(id);
            if(result!=null)
                CreateExcelFile.CreateExcelDocument(result.table, result.Name + ".xlsx", System.Web.HttpContext.Current.Response);
        }

        public void ExportOperations(int id)
        {
            //Create table for results        
            var result = _exportService.ExportOperationsForEvent(id);
            if (result != null)
                CreateExcelFile.CreateExcelDocument(result.Table, result.Name + ".xlsx", System.Web.HttpContext.Current.Response);
        }

        public ActionResult CreateUsers()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CreateUsers(HttpPostedFileBase file)
        {
            StreamReader sr = new StreamReader(file.InputStream);
            string line;
            int i = 1;
            while ((line = sr.ReadLine()) != null)
            {
                string[] user = line.Split();

                if (user.Count() != 4)
                    return Json("Погрешен формат на линија " + i + ". <br /> Сите претходни корисници се успешно додадени.");

                string username = user[0];
                string name = user[1];
                string surname = user[2];
                string password = user[3];

                var result = _userManager.Create(new ApplicationUser { UserName = username, FirstName = name, LastName = surname }, password);

                if(!result.Succeeded)
                {   
                    StringBuilder errors = new StringBuilder();

                    foreach(String s in result.Errors)
                    {
                        errors.Append("<br />");
                        errors.Append(s);
                    }

                    return Json("Следните неправилности се најдени во линија " + i + ":" + errors + "<br /> Сите претходни корисници се успешно додадени.");
                }
                i++;
            }

            return Json("Сите корисници се успешно креирани.");
        }
       
        //id == page
        public ActionResult Users(int id)
        {
            int page = id;
            var result = _userService.GetAllUsers(page, Utilities.Constants.stuffPerPage);
            int maxPages = result.TotalCount / Utilities.Constants.stuffPerPage;
            if (result.TotalCount % Utilities.Constants.stuffPerPage > 0)
                maxPages++;

            ViewBag.pages = maxPages;

            return View(result);
        }

        public ActionResult Edit(string id)
        {
            var user = _userManager.FindById(id);
            return View(new RegisterViewModel { Username = user.UserName, Ime = user.FirstName, Prezime = user.LastName, email = user.Email, ID = user.Id });
        }

        [HttpPost]
        public ActionResult Edit(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userManager.FindById(model.ID);

                user.UserName = model.Username;
                user.FirstName = model.Ime;
                user.LastName = model.Prezime;

                user.PasswordHash = _userManager.PasswordHasher.HashPassword(model.Password);

                var res = _userManager.Update(user);
                return RedirectToAction("Users", new { id = 1 });

            }
            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _userManager.Create(new ApplicationUser { FirstName = model.Ime, LastName = model.Prezime, Email = model.email, UserName = model.email }, model.Password);
                if (result.Succeeded)
                    return RedirectToAction("Users", new { id = 1 });
            }
            return View(model);
        }

        public ActionResult Events()
        {
            var events = _eventService.GetAllEvents();
            return View(events);
        }
                
        public ActionResult CreateEvent()
        {
            CreateEventViewModel model = new CreateEventViewModel();
            model.AllGroups = _groupService.GetAllGroups();
            model.AllSnippets = _snippetService.GetAllSnippets(1, 20);
            model.AllOperations = _snippetService.GetAllOperations();
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateEvent(CreateEventViewModel model)
        {
            string[] dateTime = model.date.Split('.');
            int day = Int32.Parse(dateTime[0]);
            int month = Int32.Parse(dateTime[1]);
            int year = Int32.Parse(dateTime[2]);

            DateTime start = new DateTime(year, month, day, model.hourStart, model.minStart, 0);
            DateTime end = new DateTime(year, month, day, model.hourEnd, model.minEnd, 0);

            //List<Snippet> snippets = _snippetService.GetAllSnippetsByID(model.Snippets);

            Event ev = new Event { Name = model.name, Description = model.description, Start = start, End = end/*, Snippets = snippets*/ };

            bool res = _eventService.AddOrUpdateEvent(ev, model.Snippets);

            if (!res)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

            return Json("Натпреварот е успешно креиран.", JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditEvent(int id)
        {
            EditEventViewModel model = new EditEventViewModel();
            
            model.AllGroups = _groupService.GetAllGroups();
            model.AllSnippets = _snippetService.GetAllSnippets(1, 20);
            model.AllOperations = _snippetService.GetAllOperations();
            model.Event = _eventService.GetEventById(id);
            model.Event.Snippets = model.Event.EventSnippets.OrderBy(x => x.OrderNumber).Select(x => x.Snippet).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult EditEvent(CreateEventViewModel model)
        {
            string[] dateTime = model.date.Split('.');
            int day = Int32.Parse(dateTime[0]);
            int month = Int32.Parse(dateTime[1]);
            int year = Int32.Parse(dateTime[2]);

            DateTime start = new DateTime(year, month, day, model.hourStart, model.minStart, 0);
            DateTime end = new DateTime(year, month, day, model.hourEnd, model.minEnd, 0);

            //List<Snippet> snippets = _snippetService.GetAllSnippetsByID(model.Snippets);
            Event ev = new Event { Name = model.name, Description = model.description, Start = start, End = end, ID = model.id/*, Snippets = snippets*/ };

            bool res = _eventService.AddOrUpdateEvent(ev, model.Snippets);

            if (!res)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

            return Json("Натпреварот е успешно изменет.", JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteEvent(int id)
        {
            bool res = _eventService.DeleteEvent(id);
            return RedirectToAction("Events");
        }


        public ActionResult Snippets(int page = 1)
        {
            if (page < 1)
                page = 1;

            FilterSnippetsViewModel model = new FilterSnippetsViewModel();
            model.Groups = _groupService.GetAllGroups();
            model.Operations = _snippetService.GetAllOperations();
            model.Snippets = _snippetService.GetAllSnippets(page, Utilities.Constants.stuffPerPage);
                        
            return View(model);
        }

        public ActionResult FilterSnippets(FilterSnippetsInput filterData, string view)
        {
            var snippets = _snippetService.FilterSnippets(filterData);
            ListSnippetsPartialViewModel vm = new ListSnippetsPartialViewModel
            {
                Snippets = snippets
            };

            if(view == "snippets")
            {
                vm.SnippetsButtons = true;
                vm.SpanSizeSnippets = "span9";
                vm.SpanSizeArea = "span12";
            }

            return PartialView("_ListSnippets", vm);
        }

        public ActionResult CreateSnippet()
        {
            CreateSnippetViewModel model = new CreateSnippetViewModel();
            model.Operations = _snippetService.GetAllOperations();
            model.Groups = _groupService.GetAllGroups();
            return View(model);
        }

        [HttpPost]
        public JsonResult CreateSnippet(Snippet snippet, List<OperatorsHelper> Operators, List<Group> SnippetGroups)
        {
            if (ModelState.IsValid)
            {
                bool res = _snippetService.AddOrUpdateSnippet(snippet, Operators, SnippetGroups);

                if (res)
                    return Json("Успешно зачуван снипет.", JsonRequestBehavior.AllowGet);
            }
            return Json("error", JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditSnippet(int id)
        {
            EditSnippetViewModel EditSnippetModel = new EditSnippetViewModel();

            EditSnippetModel.Snippet = _snippetService.GetSnippetById(id);
            EditSnippetModel.Operations = _snippetService.GetAllOperations();
            EditSnippetModel.Groups = _groupService.GetAllGroups();
            return View(EditSnippetModel);
        }

        public ActionResult DeleteSnippet(int id)
        {
            bool res = _snippetService.DeleteSnippet(id);
            return RedirectToAction("Snippets");
        }

        public ActionResult Groups()
        {
            var groups = _groupService.GetAllGroups();
            return View(groups);
        }

        public ActionResult Group(int id)
        {
            var _snippets = _snippetService.GetSnippetsFromGroup(id);
            var _group = _groupService.GetGroupByID(id);
            SnippetsByGroupViewModel result = new SnippetsByGroupViewModel { Group = _group, Snippets = _snippets };
            return View(result);
        }

        public ActionResult CreateGroup()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CreateGroup(string Name)
        {
            Group group = new Group {Name = Name};

            int res = _groupService.AddOrUpdateGroup(group);
            
            if(res > 0)
                return Json("success");

            return Json("error");
        }

        public ActionResult EditGroup(int id)
        {
            var model = _groupService.GetGroupByID(id);

            return View(model);
        }

        [HttpPost]
        public JsonResult EditGroup(Group group)
        {
            int res = _groupService.AddOrUpdateGroup(group);
            
            if (res > 0)
                return Json("success");

            return Json("error");
        }

        public ActionResult DeleteGroup(int id)
        {
            bool res = _groupService.DeleteGroup(id);

            return RedirectToAction("Groups");
        }

        [HttpPost]
        public JsonResult RemoveSnippetFromGroup(int SnippetID, int GroupID)
        {
            int res = _groupService.RemoveSnippetFromGroup(SnippetID, GroupID);
            return Json(res);
        }

        //id == Event id
        public ActionResult Results(int id)
        {
            List<AnswerLog> answerlog = _eventService.GetResultsForEvent(id);

            Dictionary<ApplicationUser, List<AnswerLog>> result = new Dictionary<ApplicationUser, List<AnswerLog>>();
            Dictionary<ApplicationUser, int[]> points = new Dictionary<ApplicationUser, int[]>();

            foreach (var item in answerlog)
            {
                if (!result.ContainsKey(item.User))
                {
                    result.Add(item.User, new List<AnswerLog>());
                    points.Add(item.User, new int[2]);
                }

                result[item.User].Add(item);
                points[item.User][1] += item.timeElapsed;
                if (item.isCorrect)
                {
                    points[item.User][0]++;
                }
            }

            Dictionary<ApplicationUser, int[]> finalResult = new Dictionary<ApplicationUser, int[]>();

            while (points.Count > 0)
            {
                int i = -1, t = 1000;
                ApplicationUser tmp = null;
                foreach (var item in points)
                {
                    if (item.Value[0] > i)
                    {
                        i = item.Value[0];
                        t = item.Value[1];
                        tmp = item.Key;
                    }
                    else if (item.Value[0] == i)
                    {
                        if (item.Value[1] < t)
                        {
                            i = item.Value[0];
                            t = item.Value[1];
                            tmp = item.Key;
                        }
                    }
                }

                finalResult.Add(tmp, new int[] { i, t });
                points.Remove(tmp);
            }

            //result.OrderByDescending(x => x.Value.Where(y => y.isCorrect).Count());

            return View(finalResult);
        }
    }
}
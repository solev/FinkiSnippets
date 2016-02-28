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

            CreateExcelFile.CreateExcelDocument(result.table, result.Name + ".xlsx", System.Web.HttpContext.Current.Response);
        }

        public void ExportOperations(int id)
        {
            //Create table for results        
            var result = _exportService.ExportOperationsForEvent(id);
            CreateExcelFile.CreateExcelDocument(result.Table, "Zadaci.xlsx", System.Web.HttpContext.Current.Response);
        }

        /*public ActionResult AddTestUsers()
        {
            var path = @"C:\Users\solev\Desktop\Whatever\IT_Sistemi_Users.xlsx";

            if (!System.IO.File.Exists(path))
            {
                return RedirectToAction("Users", new { id = 1 });
            }

            foreach (var worksheet in Workbook.Worksheets(path))
            {
                foreach (var sheet in worksheet.Rows.Skip(153))
                {
                    string firstName = sheet.Cells[0].Text.ToString();
                    string lastName = sheet.Cells[1].Text.ToString();
                    string index = sheet.Cells[2].Text.ToString();
                    string email = sheet.Cells[3].Text.ToString();
                    string password = index + "!";

                    ApplicationUser userTemp = new ApplicationUser { UserName = index, FirstName = firstName, LastName = lastName, Email = email };
                    var res = _userManager.Create(userTemp, password);

                    if (res.Succeeded == false)
                    {
                        return RedirectToAction("Events");
                    }
                }
            }

            //ApplicationUser testUser = new ApplicationUser { UserName = username, FirstName = fname, LastName = lname };
            //var res = userManager.Create(testUser, password);
            return RedirectToAction("Users", new { id = 1 });
        }*/

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
            return View(new RegisterViewModel { Ime = user.FirstName, Prezime = user.LastName, email = user.Email, ID = user.Id });
        }

        [HttpPost]
        public ActionResult Edit(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userManager.FindById(model.ID);

                user.FirstName = model.Ime;
                user.LastName = model.Prezime;

                var res = _userManager.Update(user);
                return RedirectToAction("Users");

            }
            return View(model);
        }

        //public ActionResult feedFalseUsers()
        //{
        //    for(int i = 1; i <= 30;i++ )
        //    {
        //        string fname = ("User" + i);
        //        string sname=" FINKI";
        //        string password = "finki123";
        //        string email = String.Format("user{0}@gmail.com",i);

        //        ApplicationUser user = new ApplicationUser { FirstName = fname, LastName = sname, Email = email, UserName = email };
        //        var res = userManager.Create(user, password);

        //    }
        //        return RedirectToAction("Users", new { id = 1 });
        //}

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

            Event ev = new Event { Name = model.name, Start = start, End = end/*, Snippets = snippets*/ };

            bool res = _eventService.AddOrUpdateEvent(ev, model.Snippets);

            if (!res)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditEvent(int id)
        {
            EditEventViewModel model = new EditEventViewModel();
            
            model.AllGroups = _groupService.GetAllGroups();
            model.AllSnippets = _snippetService.GetAllSnippets(1, 20);
            model.AllOperations = _snippetService.GetAllOperations();
            model.Event = _eventService.GetEventById(id);

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
            Event ev = new Event { Name = model.name, Start = start, End = end, ID = model.id/*, Snippets = snippets*/ };

            bool res = _eventService.AddOrUpdateEvent(ev, model.Snippets);

            if (!res)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

            return Json("success", JsonRequestBehavior.AllowGet);
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
                    return Json("Успешно зачуван снипет!", JsonRequestBehavior.AllowGet);
            }
            return Json("FAIIILLL!!!!", JsonRequestBehavior.AllowGet);
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
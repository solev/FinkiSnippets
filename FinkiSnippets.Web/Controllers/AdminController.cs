using App.Models;
using App.ViewModels;
using Data_Access;
using Data_Access.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Excel;
using ExportToExcel;
using DocumentFormat.OpenXml.Packaging;
using System.Data;
using System.Data.Entity;

namespace App.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        CodeDatabase db = new CodeDatabase();
        ApplicationUserManager userManager = new ApplicationUserManager();
        const int stuffPerPage = 10;


        public void ExportResults(int id)
        {
            //Create table for results
            DataTable dt = new DataTable();
            dt.Columns.Add("Натпревар " + DateTime.Now.ToShortDateString(), typeof(string));
            int groupID = db.Events.Where(x => x.ID == id).Include(x=>x.Group).Select(x => x.Group.ID).FirstOrDefault();
            var questions = db.Snippets.Where(x => x.Group.ID == groupID).Count();

            for (int i = 1; i <= questions; i++)
            {
                dt.Columns.Add(i.ToString(), typeof(string));
            }

            var tempEvent = db.Events.FirstOrDefault(x => x.ID == id);
            if(tempEvent == null)
                return;

           
            var users = db.Answers.Where(x => x.Event.ID == tempEvent.ID).Select(x => x.User.Id).Distinct().ToList();
            foreach(var user in users)
            {
                var answers = db.Answers.Where(x => x.Event.ID == tempEvent.ID && x.User.Id == user).ToList();
                object[] rowData = new object[answers.Count + 1];
                rowData[0] = answers[0].User.FirstName+" "+answers[0].User.LastName;                
                foreach(var item in answers)
                {
                    rowData[item.snippet.OrderNumber] = item.timeElapsed+" | "+(item.isCorrect?"Точно":"Погрешно");           
                    
                }
                dt.Rows.Add(rowData);
            }
            
            CreateExcelFile.CreateExcelDocument(dt, "Natprevar " + tempEvent.Start.ToShortDateString()+".xlsx", System.Web.HttpContext.Current.Response);            
        }

        public void ExportOperations(int id)
        {
            //Create table for results        
            int groupID = db.Events.FirstOrDefault(x => x.ID == id).Group.ID;
            var questions = db.Snippets.Where(x => x.Group.ID == groupID).ToList();
            var op = db.Operations.ToList();

            //create table for snippets and operations
            DataTable dtSnippets = new DataTable();
            dtSnippets.Columns.Add("Задача", typeof(string));
            
            foreach (var item in op)
            {
                dtSnippets.Columns.Add(item.Operator, typeof(int));
            }

            foreach (var item in questions)
            {
                object[] rowData = new object[op.Count + 1];
                var ops = db.SnippetOperations.Where(x => x.SnippetID == item.ID).Select(x => new { x.OperationID, x.Frequency });
                rowData[0] = "Задача " + item.OrderNumber;
                int idx = 1;
                foreach (var optemp in op)
                {
                    var tempOperation = ops.FirstOrDefault(x => x.OperationID == optemp.ID);
                    if (tempOperation != null)
                    {

                        rowData[idx++] = tempOperation.Frequency;
                    }
                    else
                    {
                        rowData[idx++] = 0;
                    }
                }
                dtSnippets.Rows.Add(rowData);
            }
            CreateExcelFile.CreateExcelDocument(dtSnippets, "Zadaci.xlsx", System.Web.HttpContext.Current.Response);
        } 
        
        public ActionResult CreateTestUsers()
        {
            for(int i=1;i<=30;i++)
            {
                string username = String.Format("user{0}@gmail.com", i);
                string fname = "User" + i,lname = "Test";
                string password = "123456";

                ApplicationUser us = new ApplicationUser { UserName = username, FirstName = fname, LastName = lname };
                userManager.Create(us, password);
            }
            return RedirectToAction("Users");
        }

        public ActionResult AddTestUsers()
        {
            var path = @"C:\Users\solev\Desktop\Whatever\IT_Sistemi_Users.xlsx";

            if(!System.IO.File.Exists(path))
            {
                return RedirectToAction("Users", new { id = 1 });
            }

            foreach(var worksheet in Workbook.Worksheets(path))
            {
                foreach(var sheet in worksheet.Rows.Skip(153))
                {
                    string firstName = sheet.Cells[0].Text.ToString();                    
                    string lastName = sheet.Cells[1].Text.ToString();
                    string index = sheet.Cells[2].Text.ToString();
                    string email = sheet.Cells[3].Text.ToString();
                    string password = index+"!";

                    ApplicationUser userTemp = new ApplicationUser { UserName = index, FirstName = firstName, LastName = lastName,Email = email };
                    var res = userManager.Create(userTemp, password);

                    if(res.Succeeded == false)
                    {
                        return RedirectToAction("Events");
                    }
                }
            }

            //ApplicationUser testUser = new ApplicationUser { UserName = username, FirstName = fname, LastName = lname };
            //var res = userManager.Create(testUser, password);
            return RedirectToAction("Users", new { id = 1 });
        }

        //id == page
        public ActionResult Users(int id)
        {
            int userNum = db.Users.Count();
            userNum = (int)Math.Ceiling((double)(userNum /stuffPerPage));
            ViewBag.pages = userNum;            
            var users = db.Users.OrderBy(x=>x.FirstName).Skip((id-1)*stuffPerPage).Take(stuffPerPage).ToList();
            return View(users);
        }

        public ActionResult Edit(string id)
        {

            var user = userManager.FindById(id);
            return View(new RegisterViewModel { Ime = user.FirstName, Prezime = user.LastName, email = user.Email, ID = user.Id });
        }

        [HttpPost]
        public ActionResult Edit(RegisterViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = userManager.FindById(model.ID);

                user.FirstName = model.Ime;
                user.LastName = model.Prezime;

                var res = userManager.Update(user);
                return RedirectToAction("Index");

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
            if(ModelState.IsValid)
            {
                var result = userManager.Create(new ApplicationUser { FirstName = model.Ime, LastName = model.Prezime, Email = model.email, UserName = model.email }, model.Password);
                if(result.Succeeded)
                    return RedirectToAction("Users", new { id=1});
            }
            return View(model);
        }
                
        public ActionResult CreateSnippet()
        {
            CreateSnippetViewModel model = new CreateSnippetViewModel();
            model.Operations = db.Operations.ToList();
            model.Groups = db.Groups.ToList();
            return View(model);
        }

                
        [HttpPost]
        public JsonResult CreateSnippet(Snippet snippet, List<OperatorsHelper> Operators)
        {
            int last;
            if (Operators == null)
                Operators = new List<OperatorsHelper>();
            try
            {
                last = db.Snippets.Where(x=>x.Group.ID == snippet.Group.ID).Max(x => x.OrderNumber);
            }
            catch
            {
                last = 0;
            }

            snippet.OrderNumber = last + 1;

            if(ModelState.IsValid)
            {
                var gr = db.Groups.FirstOrDefault(x => x.ID == snippet.Group.ID);
                snippet.Group = gr;
                db.Snippets.Add(snippet);
                db.SaveChanges();

                foreach(var op in Operators)
                {
                    db.SnippetOperations.Add(new SnippetOperation { Frequency = op.Frequency, OperationID = op.OperationID, SnippetID = snippet.ID });
                }

                int res = db.SaveChanges();
                if(res > 0)
                    return Json("Успешно зачуван снипет!", JsonRequestBehavior.AllowGet);
            }
            return Json("FAIIILLL!!!!", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Events()
        {
            var events = db.Events.OrderByDescending(x => x.Start).ToList();
            return View(events);
        }

        public ActionResult CreateEvent()
        {
            CreateEventViewModel model = new CreateEventViewModel();
            model.Groups = db.Groups.ToList();
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

            var evs = db.Events.Where(x => x.Start < start && x.End > start).Count();

            if(evs > 0)
            {
                return Json("error", JsonRequestBehavior.AllowGet);    
            }

            Event ev = new Event { Start = start, End = end };
            ev.Group = db.Groups.FirstOrDefault(x => x.ID == model.GroupID);
            db.Events.Add(ev);
            db.SaveChanges();

            return Json("success",JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditEvent(int id)
        {
            var ev = db.Events.FirstOrDefault(x => x.ID == id);
            string tempDate = String.Format("{0}.{1}.{2}", ev.Start.Day, ev.Start.Month, ev.Start.Year);
            
            return View(ev);
        }

        [HttpPost]
        public ActionResult EditEvent(CreateEventViewModel model)
        {
            var ev = db.Events.FirstOrDefault(x => x.ID == model.id);

            string[] dateTime = model.date.Split('.');
            int day = Int32.Parse(dateTime[0]);
            int month = Int32.Parse(dateTime[1]);
            int year = Int32.Parse(dateTime[2]);

            DateTime start = new DateTime(year, month, day, model.hourStart, model.minStart, 0);
            DateTime end = new DateTime(year, month, day, model.hourEnd, model.minEnd, 0);

            var evs = db.Events.Where(x => x.Start < start && x.End > start && x.ID!=ev.ID).Count();
            if(evs > 0)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

            ev.Start = start;
            ev.End = end;
            int res = db.SaveChanges();

            if(res > 0)
            {
                return Json("success", JsonRequestBehavior.AllowGet);
            }

            return View(model.id);
        }

        //id == Event id
        public ActionResult Results(int id)
        {
            List<AnswerLog> answerlog = db.Answers.Where(x => x.Event.ID == id).ToList();

            Dictionary<ApplicationUser,List<AnswerLog>> result = new Dictionary<ApplicationUser,List<AnswerLog>>();
            Dictionary<ApplicationUser, int[]> points = new Dictionary<ApplicationUser,int[]>();

            foreach(var item in answerlog)
            {
                if(!result.ContainsKey(item.User))
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

            while(points.Count > 0)
            {
                int i = -1,t=1000;
                ApplicationUser tmp = null;
                foreach(var item in points)
                {
                    if(item.Value[0]>i)
                    {
                        i = item.Value[0];
                        t=item.Value[1];
                        tmp = item.Key;
                    }
                    else if(item.Value[0] == i)
                    {
                        if(item.Value[1] < t)
                        {
                            i = item.Value[0];
                            t = item.Value[1];
                            tmp = item.Key;
                        }
                    }
                }
                
                finalResult.Add(tmp, new int[]{i,t});
                points.Remove(tmp);
            }            

            //result.OrderByDescending(x => x.Value.Where(y => y.isCorrect).Count());

            return View(finalResult);
        }




	}


    
}
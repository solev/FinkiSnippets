using App.Models;
using App.ViewModels;
using Data_Access;
using Data_Access.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Controllers
{
    [Authorize]
    public class CodeController : Controller
    {

        CodeDatabase db = new CodeDatabase();

        public static DateTime getMyTime()
        {
            return DateTime.Now.AddHours(1);
        }       
        

        public ActionResult Start()
        {
            DateTime t = DateTime.Now.AddHours(1);
            
            try
            {
            
                var ev = db.Events.Where(x => x.End > t).OrderBy(x=>x.Start).Take(1).Single();
                return View(ev);
            }
            catch
            {
                return View();
            }
            
        }

        //id == orderNumber
        public ActionResult Game()
        {
            DateTime t = DateTime.Now.AddHours(1);

            var ev = db.Events.FirstOrDefault(x => x.Start < t && x.End > t);
            if(ev==null)
                return RedirectToAction("Start");
            
            AnswerLog last = null;

            try
            {
                last = db.Answers.Where(x => x.User.UserName == User.Identity.Name && x.Event.ID == ev.ID && x.answered && x.snippet.Group.ID == ev.Group.ID).OrderByDescending(x => x.snippet.OrderNumber).Take(1).Single();
            }catch{
                last = null;
            }

            int id=1;
            if(last != null)
                id = last.snippet.OrderNumber + 1;
            
            Snippet snippet;
            int lastOrderNumber = db.Snippets.Where(x => x.Group.ID == ev.Group.ID).OrderByDescending(x => x.ID).Take(1).Single().OrderNumber;
            if (id > lastOrderNumber)
            {
                return RedirectToAction("Result", new { status = "YzK12QQu" });
            }

            snippet = db.Snippets.FirstOrDefault(x => x.OrderNumber == id && x.Group.ID == ev.Group.ID);

            var check = db.Answers.FirstOrDefault(x => x.User.UserName == User.Identity.Name && x.snippet.ID == snippet.ID && x.Event.ID == ev.ID);

            if(check==null)
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

            var snippetTemp = db.Snippets.FirstOrDefault(x => x.ID == id);
            var userT = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            var ev = db.Events.FirstOrDefault(x => x.Start < t && x.End > t);

            if(snippetTemp != null && userT != null && ev!=null)
            {
                var oldAns = db.Answers.FirstOrDefault(x => x.snippet.ID == snippetTemp.ID && x.User.Id == userT.Id && ev.ID == x.Event.ID);
                var timeElapsed = (int)(t - oldAns.DateCreated).TotalSeconds;

                if(snippetTemp.Output == answer)
                {
                    oldAns.isCorrect = true;
                }
                                
                oldAns.timeElapsed = timeElapsed;
                oldAns.answered = true;
                var res = db.SaveChanges();
                return RedirectToAction("Game");
            }

            return RedirectToAction("Game");
        }

        public ActionResult Result(string status)
        {
            if(status!=null && status != "YzK12QQu")
                return RedirectToAction("Game");
            return View();
        }

        //[Authorize(Roles = "Admin")]
        //public void FeedOperators()
        //{
        //    db.Operations.Add(new Operation { Name = "Plus", Operator = "+" });
        //    db.Operations.Add(new Operation { Name = "Minus", Operator = "-" });
        //    db.Operations.Add(new Operation { Name = "Multiply", Operator = "*" });
        //    db.Operations.Add(new Operation { Name = "Devide", Operator = "/" });
        //    db.Operations.Add(new Operation { Name = "Modulo", Operator = "%" });
        //    db.Operations.Add(new Operation { Name = "Equals", Operator = "==" });
        //    db.Operations.Add(new Operation { Name = "Not Equal", Operator = "!=" });
        //    db.Operations.Add(new Operation { Name = "LT", Operator = "<" });
        //    db.Operations.Add(new Operation { Name = "LTE", Operator = "<=" });
        //    db.Operations.Add(new Operation { Name = "GT", Operator = ">" });
        //    db.Operations.Add(new Operation { Name = "GTE", Operator = ">=" });
        //    db.Operations.Add(new Operation { Name = "Not", Operator = "!" });
        //    db.Operations.Add(new Operation { Name = "And", Operator = "&&" });
        //    db.Operations.Add(new Operation { Name = "Or", Operator = "||" });

        //    db.SaveChanges();
        //}

    }
}
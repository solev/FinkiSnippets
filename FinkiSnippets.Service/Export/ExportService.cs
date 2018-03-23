using FinkiSnippets.Data;
using FinkiSnippets.Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data;

namespace FinkiSnippets.Service
{
    public class ExportService : IExportService
    {
        private CodeDatabase db;

        public ExportService(CodeDatabase _db)
        {
            db = _db;
        }

        public ExportResultsDto ExportResultsForEvent(int eventID)
        {

            var ev = db.Events.Where(x => eventID == x.ID).Select(x => new
            {
                x.ID,
                x.Name,
                Snippets = x.EventSnippets.Select(y => y.OrderNumber)
            }).FirstOrDefault();

            if (ev == null)
                return null;

            var tempres = db.UserEvents.Where(x => x.EventID == eventID).Include(x => x.User.Answers).Select(x => new
            {
                x.UserID,
                x.User.UserName,
                x.User.FirstName,
                x.User.LastName,
                Answers = x.User.Answers.Where(y => y.EventID == eventID).Select(y => new
                {
                    y.isCorrect,
                    y.timeElapsed
                })
            }).ToList();


            DataTable dt = new DataTable();
            dt.Columns.Add("Username", typeof(string));
            dt.Columns.Add("Име", typeof(string));


            foreach (var item in ev.Snippets)
            {
                dt.Columns.Add(item.ToString()+" (Time)");
                dt.Columns.Add(item.ToString() + " (Correctness)");
            }


            foreach (var user in tempres)
            {
                object[] rowdata = new object[user.Answers.Count() * 2 + 2];
                rowdata[0] = user.UserName;
                rowdata[1] = string.Format("{0} {1}", user.FirstName, user.LastName);

                int i = 2;

                foreach (var item in user.Answers)
                {
                    rowdata[i] = item.timeElapsed;
                    rowdata[i + 1] = item.isCorrect ? 1 : 0;
                    i += 2;
                }

                dt.Rows.Add(rowdata);
            }

            ExportResultsDto result = new ExportResultsDto();
            result.table = dt;
            result.Name = ev.Name;

            return result;
        }


        public ExportOperationsDto ExportOperationsForEvent(int eventID)
        {

            var ev = db.Events.Where(x => x.ID == eventID).Include(x => x.EventSnippets).Include(x=>x.EventSnippets.Select(y=>y.Snippet.Operations)).Select(x => new
            {
                
                x.Name,
                Snippets = x.EventSnippets.Select(y => new
                {
                    y.OrderNumber,
                    Operations = y.Snippet.Operations
                })
            }).FirstOrDefault();

            if (ev == null)
                return null;

            var operations = db.Operations.ToList();


            //create table for snippets and operations
            DataTable dtSnippets = new DataTable();
            dtSnippets.Columns.Add("Задача", typeof(string));

            foreach (var item in operations)
            {
                dtSnippets.Columns.Add(item.Operator);
            }

            foreach (var item in ev.Snippets)
            {
                object[] rowData = new object[operations.Count + 1];                
                rowData[0] = "Задача " + item.OrderNumber;
                int idx = 1;
                foreach (var optemp in operations)
                {
                    var tempOperation = item.Operations.FirstOrDefault(x => x.OperationID == optemp.ID);
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

            ExportOperationsDto result = new ExportOperationsDto();
            result.Table = dtSnippets;
            result.Name = string.Format("{0} - Operacii", ev.Name);
            return result;
        }


        public ExportResultsDto ExportResultsForEventOld(int eventID)
        {           
            DataTable dt = new DataTable();

            dt.Columns.Add("Натпревар " + DateTime.Now.ToShortDateString(), typeof(string));

            var evt = db.Events.Where(x => x.ID == eventID)
                .Include(x => x.Answers)                
                .FirstOrDefault();

            if (evt == null)
                return null;

            for (int i = 1; i <= evt.Answers.Count; i++)
            {
                dt.Columns.Add(i.ToString(), typeof(string));
            }            
            
            var users = evt.Answers.Select(x=>x.UserID).Distinct().ToList();

            foreach (var user in users)
            {
                var User = db.Users.Find(user);
                var answers = evt.Answers.Where(x => x.UserID == user).ToList();

                object[] rowData = new object[answers.Count + 1];
                rowData[0] = User.FirstName + " " + User.LastName;

                int ct = 1;
                foreach (var item in answers)
                {  
                    rowData[ct] = item.timeElapsed + " | " + (item.isCorrect ? "Точно" : "Погрешно");
                    ct++;
                }

                dt.Rows.Add(rowData);
            }

            ExportResultsDto result = new ExportResultsDto();
            result.table = dt;
            result.Name = "natprevar_" + evt.Start.ToShortDateString();

            return result;
        }

        public ExportOperationsDto ExportOperationsForEventOld(int eventID)
        {
            var snippets = db.Answers.Where(x=>x.EventID == eventID)
                .Select(x=>x.SnippetID)              
                .ToList();

            //var op = db.Operations.ToList();
            //var snippetoperators = db.SnippetOperations.Where(x => snippets.Contains(x.SnippetID)).Include(x=>x.Operation).ToList();

            ////create table for snippets and operations
            DataTable dtSnippets = new DataTable();
            dtSnippets.Columns.Add("Задача", typeof(string));

            //foreach (var item in op)
            //{
            //    dtSnippets.Columns.Add(item.Operator, typeof(int));
            //}
            //int ct = 1;
            //foreach (var item in snippetoperators)
            //{
            //    object[] rowData = new object[op.Count + 1];
            //    var ops = snippetoperators.ToList();
            //    rowData[0] = "Задача " + ct;
            //    int idx = 1;
            //    foreach (var optemp in op)
            //    {
            //        var tempOperation = ops.FirstOrDefault(x => x.OperationID == optemp.ID);
            //        if (tempOperation != null)
            //        {

            //            rowData[idx++] = tempOperation.Frequency;
            //        }
            //        else
            //        {
            //            rowData[idx++] = 0;
            //        }
            //    }
            //    ct++;
            //    dtSnippets.Rows.Add(rowData);
            //}

            ExportOperationsDto result = new ExportOperationsDto();
            ////result.Table = dtSnippets;

            return result;
        }
    }
}

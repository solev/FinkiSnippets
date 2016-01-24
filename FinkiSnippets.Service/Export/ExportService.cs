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
            //var tempResult = db.Events.Where(x => x.ID == eventID).Include(x => x.Answers.Select(y => y.User)).SelectMany(x => x.Answers).Select(x => new
            //{ 
            //    x.User.FirstName,
            //    x.User.LastName,
            //    x.isCorrect,
            //    x.timeElapsed                
            //});

            DataTable dt = new DataTable();
            dt.Columns.Add("Натпревар " + DateTime.Now.ToShortDateString(), typeof(string));
            int groupID = db.Events.Where(x => x.ID == eventID).Include(x => x.Group).Select(x => x.Group.ID).FirstOrDefault();
            var questions = db.Snippets.Where(x => x.Group.ID == groupID).Count();

            for (int i = 1; i <= questions; i++)
            {
                dt.Columns.Add(i.ToString(), typeof(string));
            }

            var tempEvent = db.Events.FirstOrDefault(x => x.ID == eventID);
            if (tempEvent == null)
                return null;


            var users = db.Answers.Where(x => x.Event.ID == tempEvent.ID).Select(x => x.User.Id).Distinct().ToList();
            foreach (var user in users)
            {
                var answers = db.Answers.Where(x => x.Event.ID == tempEvent.ID && x.User.Id == user).ToList();
                object[] rowData = new object[answers.Count + 1];
                rowData[0] = answers[0].User.FirstName + " " + answers[0].User.LastName;
                foreach (var item in answers)
                {
                    rowData[item.snippet.OrderNumber] = item.timeElapsed + " | " + (item.isCorrect ? "Точно" : "Погрешно");

                }
                dt.Rows.Add(rowData);
            }

            ExportResultsDto result = new ExportResultsDto();
            result.table = dt;
            result.Name = "Natprevar_"+ tempEvent.Start.ToShortDateString();

            return result;
        }


        public ExportOperationsDto ExportOperationsForEvent(int eventID)
        {
            int groupID = db.Events.Where(x => x.ID == eventID).Select(x => x.Group.ID).FirstOrDefault();
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

            ExportOperationsDto result = new ExportOperationsDto();
            result.Table = dtSnippets;

            return result;
        }
    }
}

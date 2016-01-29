using Entity;
using System.Data.Entity;
using FinkiSnippets.Data;
using FinkiSnippets.Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace FinkiSnippets.Service
{
    public class SnippetService : ISnippetService
    {

        private CodeDatabase db;

        public SnippetService(CodeDatabase _db)
        {
            db = _db;
        }

        public bool SubmitAnswer(string userID, int SnippetID, string Answer)
        {

            DateTime currentTime = DateHelper.GetCurrentTime();

            var snippetTemp = db.Snippets.Find(SnippetID);            
            var ev = db.Events.FirstOrDefault(x => x.Start < currentTime && x.End > currentTime);

            if (snippetTemp != null && ev != null)
            {
                var initialAnswer = db.Answers.FirstOrDefault(x => x.snippet.ID == snippetTemp.ID && x.User.Id == userID && ev.ID == x.Event.ID);

                if (initialAnswer == null)
                    return false;

                var timeElapsed = (int)(currentTime - initialAnswer.DateCreated).TotalSeconds;

                if (snippetTemp.Output == Answer)
                {
                    initialAnswer.isCorrect = true;
                }

                initialAnswer.timeElapsed = timeElapsed;
                initialAnswer.answered = true;
                var res = db.SaveChanges();
                return res > 0;
            }

            return false;
        }

        public int GetLastAnsweredSnippetOrderNumber(string userID, int EventID, int GroupID)
        {
            var result = db.Answers.Where(x => x.User.Id == userID && x.Event.ID == EventID && x.answered && x.snippet.Group.ID == GroupID).OrderByDescending(x => x.snippet.OrderNumber).Select(x => x.snippet.OrderNumber).Take(1).FirstOrDefault();

            return result;
        }

        public int GetLastSnippetOrderNumber(int GroupID)
        {
            var result = db.Snippets.Where(x => x.Group.ID == GroupID).OrderByDescending(x => x.ID).Select(x => x.OrderNumber).Take(1).FirstOrDefault();

            return result;
        }

        public Snippet GetSnippetWithOrderNumber(int OrderNumber, int GroupID)
        {
            var result = db.Snippets.FirstOrDefault(x => x.OrderNumber == OrderNumber && x.Group.ID == GroupID);
            return result;
        }

        public bool CheckIfFirstSnippetAccess(string userID, int snippetID, int EventID)
        {
            var result = db.Answers.Any(x => x.User.UserName == userID && x.snippet.ID == snippetID && x.Event.ID == EventID);
            return result;
        }


        public bool CreateInitialAnswer(AnswerLog answer)
        {
            db.Answers.Add(new AnswerLog { DateCreated = answer.DateCreated,User = answer.User,snippet = answer.snippet,Event = answer.Event});
            int res = db.SaveChanges();
            return res > 0;
        }


        public bool CreateSnippet(Snippet snippet, List<OperatorsHelper> Operators)
        {
            int last;
            if (Operators == null)
                Operators = new List<OperatorsHelper>();

            

            var gr = db.Groups.FirstOrDefault(x => x.ID == snippet.Group.ID);
            
            if(snippet.ID > 0)
            {
                var snippetToChange = db.Snippets.Find(snippet.ID);

                snippetToChange.Output = snippet.Output;
                snippetToChange.Question = snippet.Question;
                snippetToChange.Code = snippet.Code;
                snippetToChange.Group = gr;

                var oldOperations = db.SnippetOperations.Where(x => x.SnippetID == snippet.ID);
                db.SnippetOperations.RemoveRange(oldOperations);                
            }
            else
            {
                try
                {
                    last = db.Snippets.Where(x => x.Group.ID == snippet.Group.ID).Max(x => x.OrderNumber);
                }
                catch
                {
                    last = 0;
                }
                snippet.Group = gr;
                snippet.OrderNumber = last + 1;
                db.Snippets.Add(snippet);
                db.SaveChanges();
            }            

            foreach (var op in Operators)
            {
                db.SnippetOperations.Add(new SnippetOperation { Frequency = op.Frequency, OperationID = op.OperationID, SnippetID = snippet.ID });
            }

            int res = db.SaveChanges();
            return res > 0;
        }
        
        public List<Operation> GetAllOperations()
        {
            var result = db.Operations.ToList();
            return result;
        }

        public List<Snippet> GetAllSnippets(int page, int snippetsPerPage)
        {
            var result = db.Snippets.ToList();
            return result;
        }
        

        public List<Snippet> GetAllCodes()
        {
            var tempResult = db.Snippets.Select(x => new { x.ID, x.Code }).ToList();

            var result = tempResult.Select(x => new Snippet { ID = x.ID, Code = x.Code }).ToList();
            return result;
        }


        public Snippet GetSnippetById(int snippetID)
        {
            var snippet = db.Snippets.Where(x => x.ID == snippetID).Include(x=>x.Group).FirstOrDefault();
            var operations = db.SnippetOperations.Where(x => x.SnippetID == snippet.ID).ToList();
            snippet.Operations = operations;

            return snippet;
        }
        
        public bool DeleteSnippet(int snippetID)
        {
            var snippet = db.Snippets.Find(snippetID);
            var operations = db.SnippetOperations.Where(x => x.SnippetID == snippetID);
            var answers = db.Answers.Where(x => x.snippet.ID == snippetID);
            db.Snippets.Remove(snippet);
            db.SnippetOperations.RemoveRange(operations);
            db.Answers.RemoveRange(answers);
            int res = db.SaveChanges();
            return res > 0;
        }

        public List<Snippet> GetSnippetsFromGroup(int groupID)
        {
            var tempResult = db.Snippets.Where(x => x.Group.ID == groupID).Select(x => new { x.ID, x.Question, x.Code }).ToList();
            var result = tempResult.Select(x => new Snippet {ID = x.ID, Question = x.Question, Code = x.Code}).ToList();
            return result;
        }
    }
}

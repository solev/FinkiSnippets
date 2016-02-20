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

        //TO DO: Create new logic
        public int GetLastAnsweredSnippetOrderNumber(string userID, int EventID, int GroupID)
        {
            //var result = db.Answers.Where(x => x.User.Id == userID && x.Event.ID == EventID && x.answered && x.snippet.Group.ID == GroupID).OrderByDescending(x => x.snippet.OrderNumber).Select(x => x.snippet.OrderNumber).Take(1).FirstOrDefault();

            //return result;

            return -1;
        }

        //TO DO: Create new Logic
        public int GetLastSnippetOrderNumber(int GroupID)
        {
            //var result = db.Snippets.Where(x => x.Group.ID == GroupID).OrderByDescending(x => x.ID).Select(x => x.OrderNumber).Take(1).FirstOrDefault();

            //return result;
            return -1;
        }

        public Snippet GetSnippetWithOrderNumber(int OrderNumber, int GroupID)
        {
            //var result = db.Snippets.FirstOrDefault(x => x.OrderNumber == OrderNumber && x.Group.ID == GroupID);
            //return result;
            return null;
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

        //TO DO: Snippet can be in multiple groups
        public bool AddOrUpdateSnippet(Snippet snippet, List<OperatorsHelper> Operators, List<Group> Groups)
        {
            int res;
            List<Group> newGroups = new List<Group>();

            if (Operators == null)
                Operators = new List<OperatorsHelper>();

            if (Groups == null)
                Groups = new List<Group>();
            else
            {
                List<int> groupIDs = new List<Int32>();

                foreach (Group g in Groups)
                {
                    groupIDs.Add(g.ID);
                }

                newGroups = db.Groups.Where(x => groupIDs.Contains(x.ID)).ToList();
            }
           
            
            if(snippet.ID > 0)
            {
                var snippetToChange = db.Snippets.Where(x => x.ID == snippet.ID).Include(x => x.Groups).FirstOrDefault();

                snippetToChange.Output = snippet.Output;
                snippetToChange.Question = snippet.Question;
                snippetToChange.Code = snippet.Code;
                snippetToChange.Groups.Clear();
                res = db.SaveChanges();

                if (newGroups.Count > 0)
                {
                    snippetToChange.Groups = newGroups;
                    res = db.SaveChanges();
                }

                var oldOperations = db.SnippetOperations.Where(x => x.SnippetID == snippet.ID).ToList();
                if (oldOperations.Count > 0)
                {
                    db.SnippetOperations.RemoveRange(oldOperations);
                    res = db.SaveChanges();
                }
            }
            else
            {
                snippet.Groups = newGroups;
                db.Snippets.Add(snippet);
                res = db.SaveChanges();
            }

            if (Operators.Count > 0)
            {
                foreach (var op in Operators)
                {
                    db.SnippetOperations.Add(new SnippetOperation { Frequency = op.Frequency, OperationID = op.OperationID, SnippetID = snippet.ID });
                }

                res = db.SaveChanges();
            }

            return res > 0;
        }
        
        public List<Operation> GetAllOperations()
        {
            var result = db.Operations.ToList();
            return result;
        }

        public List<Snippet> GetAllSnippets(int page, int snippetsPerPage)
        {
            var result = db.Snippets.OrderByDescending(x=>x.ID).Skip((page-1)*snippetsPerPage).Take(snippetsPerPage).ToList();
            //var result = db.Snippets.OrderByDescending(x => x.ID).ToList();
            return result;
        }

        public Snippet GetSnippetById(int snippetID)
        {
            var snippet = db.Snippets.Where(x => x.ID == snippetID).Include(x => x.Groups).FirstOrDefault();
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
            var tempResult = db.Groups.Where(x => x.ID == groupID).Include(x => x.Snippets).SelectMany(x => x.Snippets).Select(x=> new { x.ID,x.Question,x.Code}).OrderByDescending(x => x.ID).ToList();
            var result = tempResult.Select(x => new Snippet {ID = x.ID, Question = x.Question, Code = x.Code}).ToList();
            return result;
        }

        public List<Snippet> GetAllSnippetsByID(List<int> IDs)
        {
            List<Snippet> snippets = db.Snippets.Where(x => IDs.Contains(x.ID)).ToList();

            return snippets;
        }


        public List<Snippet> FilterSnippets(FilterSnippetsInput input)
        {
            return null;
        }
    }
}

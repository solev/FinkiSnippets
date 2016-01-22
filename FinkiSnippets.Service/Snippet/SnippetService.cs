using Entity;
using FinkiSnippets.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace FinkiSnippets.Service
{
    public class SnippetService : ISnippetService{

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
            db.Answers.Add(answer);
            int res = db.SaveChanges();
            return res > 0;
        }
    }
}

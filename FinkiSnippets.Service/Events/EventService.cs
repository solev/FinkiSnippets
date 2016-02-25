using Entity;
using System.Data.Entity;
using FinkiSnippets.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using FinkiSnippets.Service.Dto;

namespace FinkiSnippets.Service
{
    public class EventService : IEventService
    {

        private CodeDatabase db;

        public EventService(CodeDatabase _db)
        {
            db = _db;
        }

        public Event GetEventById(int eventID)
        {
            return db.Events.FirstOrDefault(x => x.ID == eventID);
        }

        public Event GetNextEvent()
        {
            DateTime currentTime = DateHelper.GetCurrentTime();
            var result = db.Events.Where(x => x.End > currentTime)
                .OrderBy(x => x.Start).Take(1).FirstOrDefault();

            return result;
        }

        public Event GetCurrentEvent()
        {
            DateTime currentTime = DateHelper.GetCurrentTime();
            var result = db.Events.FirstOrDefault(x => x.Start < currentTime && x.End > currentTime);

            return result;
        }

        public List<EventDto> GetAllEvents()
        {
            var tempResult = db.Events.OrderByDescending(x => x.Start).Select(x => new
            {
                x.ID,
                x.Start,
                x.End
            }).ToList();


            List<EventDto> result = tempResult.Select(x => new EventDto { ID = x.ID, Start = x.Start, End = x.End }).ToList();
            return result;
        }

        public bool AddOrUpdateEvent(Event ev)
        {
            int res;

            if(ev.ID > 0)
            {
                Event eventToUpdate = db.Events.Where(x => x.ID == ev.ID).Include(x=> x.Snippets).FirstOrDefault();
                eventToUpdate.Snippets.Clear();
                res = db.SaveChanges();
                eventToUpdate.Start = ev.Start;
                eventToUpdate.End = ev.End;
                eventToUpdate.Snippets = ev.Snippets;
                res = db.SaveChanges();
            }
            else
            {
                ev.Name = "TEST EVENT";
                db.Events.Add(ev);
                res = db.SaveChanges();
            }
            
            return res > 0;
        }

        public List<AnswerLog> GetResultsForEvent(int eventID)
        {
            var result = db.Answers.Where(x => x.Event.ID == eventID).Include(x => x.User).ToList();
            return result;
        }
    }
}

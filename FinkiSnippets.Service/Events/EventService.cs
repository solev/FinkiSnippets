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
            return db.Events.Where(x => x.ID == eventID).Include(x => x.SnippetEvents).FirstOrDefault();
        }

        public List<Event> GetNextEvents()
        {
            DateTime CurrentTime = DateHelper.GetCurrentTime();
            return db.Events.Where(x => x.Start > CurrentTime).ToList();
        }

        public List<Event> GetActiveEvents()
        {
            DateTime CurrentTime = DateHelper.GetCurrentTime();
            return db.Events.Where(x => x.Start < CurrentTime && x.End > CurrentTime).ToList();
        }

        public List<EventDto> GetAllEvents()
        {
            var tempResult = db.Events.OrderByDescending(x => x.Start).Select(x => new
            {
                x.ID,
                x.Name,
                x.Start,
                x.End
            }).ToList();

            List<EventDto> result = tempResult.Select(x => new EventDto { ID = x.ID, Name = x.Name, Start = x.Start, End = x.End }).ToList();
            return result;
        }

        public bool AddOrUpdateEvent(Event ev, List<Int32> IDs)
        {
            int res;

            ev.Snippets = db.Snippets.Where(x => IDs.Contains(x.ID)).ToList();

            if(ev.ID > 0)
            {
                Event eventToUpdate = db.Events.Where(x => x.ID == ev.ID).Include(x=> x.SnippetEvents).FirstOrDefault();
                eventToUpdate.SnippetEvents.Clear();
                res = db.SaveChanges();
                eventToUpdate.Name = ev.Name;
                eventToUpdate.Start = ev.Start;
                eventToUpdate.End = ev.End;
                eventToUpdate.SnippetEvents = ev.SnippetEvents;
                res = db.SaveChanges();
            }
            else
            {
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

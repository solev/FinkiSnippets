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
            return db.Events.Where(x => x.ID == eventID).Include(x => x.EventSnippets.Select(y=>y.Snippet)).FirstOrDefault();
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

            List<EventSnippets> snippetEvents = new List<EventSnippets>();
            for(int i=0;i<IDs.Count;i++)
            {
                EventSnippets snippetEvent = new EventSnippets
                {
                    SnippetID = IDs[i],
                    OrderNumber = i+1
                };
                snippetEvents.Add(snippetEvent);
            }
                        

            if(ev.ID > 0)
            {
                Event eventToUpdate = db.Events.Where(x => x.ID == ev.ID).Include(x=> x.EventSnippets).FirstOrDefault();
                db.EventSnippets.RemoveRange(eventToUpdate.EventSnippets);
                res = db.SaveChanges();
                eventToUpdate.Name = ev.Name;
                eventToUpdate.Description = ev.Description;
                eventToUpdate.Start = ev.Start;
                eventToUpdate.End = ev.End;
                snippetEvents.ForEach(x => x.EventID = ev.ID);
                eventToUpdate.EventSnippets = snippetEvents;
                res = db.SaveChanges();
            }
            else
            {
                ev.EventSnippets = snippetEvents;
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

using Entity;
using FinkiSnippets.Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinkiSnippets.Service
{
    public interface IEventService
    {
        List<Event> GetNextEvents();
        List<Event> GetActiveEvents();
        Event GetEventById(int eventID);
        List<EventDto> GetAllEvents();
        bool AddOrUpdateEvent(Event ev, List<Int32> IDs);
        List<AnswerLog> GetResultsForEvent(int eventID);
        void FinishEventForUser(int EventID, string UserID);
    }
}

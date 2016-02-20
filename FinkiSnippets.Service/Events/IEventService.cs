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
        Event GetNextEvent();
        Event GetCurrentEvent();
        Event GetEventById(int eventID);
        List<EventDto> GetAllEvents();
        bool AddOrUpdateEvent(Event ev);
        List<AnswerLog> GetResultsForEvent(int eventID);
    }
}

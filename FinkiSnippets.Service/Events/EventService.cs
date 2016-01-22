﻿using Entity;
using FinkiSnippets.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace FinkiSnippets.Service
{
    public class EventService : IEventService
    {

        private CodeDatabase db;

        public EventService(CodeDatabase _db)
        {
            db = _db;
        }

        public Event GetCurrentEvent()
        {
            DateTime currentTime = DateHelper.GetCurrentTime();
            var result = db.Events.Where(x => x.End > currentTime).OrderBy(x => x.Start).Take(1).FirstOrDefault();

            return result;
        }


        public List<Event> GetCurrentEvents()
        {
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Entity
{
    public class Event
    {
        public int ID { get; set; }
        //public string Name { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public virtual Group Group { get; set; }
    }
}
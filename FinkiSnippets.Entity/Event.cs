using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Entity
{
    public class Event
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public virtual ICollection<Snippet> Snippets { get; set; }
        public virtual ICollection<AnswerLog> Answers { get; set; }
    }
}
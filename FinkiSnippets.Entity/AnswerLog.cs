using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Entity
{
    public class AnswerLog
    {
        public int ID { get; set; }
        
        public int timeElapsed { get; set; }
        public bool isCorrect { get; set; } 
        public DateTime DateCreated { get; set; }
        public bool answered { get; set; }

        public int SnippetID { get; set; }
        public string UserID { get; set; }
        public int EventID { get; set; }

        [ForeignKey("SnippetID")]
        public virtual Snippet Snippet { get; set; }

        [ForeignKey("UserID")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("EventID")]
        public virtual Event Event { get; set; }
    }
}
using Data_Access.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Models
{
    public class AnswerLog
    {
        public int ID { get; set; }
        
        public int timeElapsed { get; set; }
        public bool isCorrect { get; set; }        
        public virtual Event  Event { get; set; }
        public DateTime DateCreated { get; set; }
        public bool answered { get; set; }

        public virtual Snippet snippet { get; set; }
        public virtual ApplicationUser User { get; set; }
                
    }
}
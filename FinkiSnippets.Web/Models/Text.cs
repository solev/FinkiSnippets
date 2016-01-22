using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Models
{
    public class Text
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public virtual Snippet snippet{ get; set; }
    }
}
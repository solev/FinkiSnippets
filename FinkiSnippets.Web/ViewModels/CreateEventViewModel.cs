using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.ViewModels
{
    public class CreateEventViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string date { get; set; }
        public int hourStart { get; set; }
        public int minStart { get; set; }
        public int hourEnd { get; set; }
        public int minEnd { get; set; }

        public virtual List<Int32> Snippets{ get; set; }
        public virtual List<Snippet> AllSnippets { get; set; }
        public virtual List<Group> AllGroups { get; set; }
        public virtual List<Operation> AllOperations { get; set; }
    }
}
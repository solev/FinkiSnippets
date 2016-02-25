using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.ViewModels
{
    public class EditEventViewModel
    {
        public Event Event{ get; set; }
        public virtual List<Snippet> AllSnippets { get; set; }
        public virtual List<Group> AllGroups { get; set; }
        public virtual List<Operation> AllOperations { get; set; }
    }
}
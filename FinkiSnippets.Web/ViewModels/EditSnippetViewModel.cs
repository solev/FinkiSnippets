using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.ViewModels
{
    public class EditSnippetViewModel
    {
        public Snippet Snippet{ get; set; }
        public List<Operation> Operations { get; set; }
        public List<Group> Groups { get; set; }
    }
}
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.ViewModels
{
    public class FilterSnippetsViewModel
    {
        public List<Snippet> Snippets { get; set; }
        public List<Group> Groups{ get; set; }
        public List<Operation> Operations { get; set; }

        public List<int> SelectedGroups { get; set; }
        public List<int> SelectedOperations { get; set; }
    }
}
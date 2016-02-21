using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;

namespace App.ViewModels
{
    public class SnippetsByGroupViewModel
    {
        public Group Group { get; set; }
        public List<Snippet> Snippets { get; set; }
    }
}
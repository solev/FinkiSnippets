using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;

namespace App.ViewModels
{
    public class SnippetsByGroupViewModel
    {
        public Group group { get; set; }
        public List<Snippet> snippets { get; set; }
    }
}
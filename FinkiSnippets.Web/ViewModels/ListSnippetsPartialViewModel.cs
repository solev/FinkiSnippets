using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.ViewModels
{
    public class ListSnippetsPartialViewModel
    {
        public List<Snippet> Snippets { get; set; }

        public bool EditRemoveButtons { get; set; }

        public string SpanSizeSnippets {get; set; }
    }
}
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.ViewModels
{
    public class ListSnippetsPartialViewModel
    {
        public int StartCounter { get; set; }

        public string DivName { get; set; }

        public List<Snippet> Snippets { get; set; }

        public bool SnippetsButtons { get; set; }

        public bool GroupButtons { get; set; }

        public string SpanSizeSnippets {get; set; }

        public string SpanSizeArea { get; set; }
    }
}
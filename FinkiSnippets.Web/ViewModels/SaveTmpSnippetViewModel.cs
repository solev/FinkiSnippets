using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using FinkiSnippets.Entity;

namespace App.ViewModels
{
    public class SaveTmpSnippetViewModel
    {
        public TemporarySnippet TmpSnippet { get; set; }
        public List<Operation> Operations { get; set; }
        public List<Group> Groups { get; set; }
    }
}
using App.Models;
using Data_Access.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.ViewModels
{
    public class CreateSnippetViewModel
    {
        public List<Operation> Operations{ get; set; }
        public List<Group> Groups{ get; set; }
    }
}
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
        public List<Group> Groups { get; set; }
    }
}
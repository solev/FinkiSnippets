using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.ViewModels
{
    public class StartViewModel
    {
        public List<Event> NextEvents { get; set; }
        public List<Event> ActiveEvents { get; set; }
    }
}
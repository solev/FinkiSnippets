using App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.ViewModels
{
    public class CreateEventViewModel
    {
        public int id { get; set; }

        public string date { get; set; }
        public int hourStart { get; set; }
        public int minStart { get; set; }
        public int hourEnd { get; set; }
        public int minEnd { get; set; }
        public int GroupID { get; set; }

        public List<Group> Groups{ get; set; }


    }
}
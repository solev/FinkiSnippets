using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Models
{
    public class SnippetOperation
    {
        public int ID { get; set; }
        public int SnippetID{ get; set; }
        public int OperationID { get; set; }
        public int Frequency { get; set; }
    }
}
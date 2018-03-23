using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Entity
{
    public class SnippetOperation
    {
        public int ID { get; set; }
        public int SnippetID{ get; set; }
        //[ForeignKey("SnippetID")]
        //public virtual Snippet Snippet { get; set; }

        public int OperationID { get; set; }
        //[ForeignKey("OperationID")]
        //public virtual Operation Operation { get; set; }

        public int Frequency { get; set; }
    }
}
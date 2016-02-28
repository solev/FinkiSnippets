using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class SnippetEvents
    {
        public int SnippetID { get; set; }
        [ForeignKey("SnippetID")]
        public virtual Snippet Snippet { get; set; }

        public int EventID { get; set; }
        [ForeignKey("EventID")]
        public virtual Event Event { get; set; }

        public int OrderNumber { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Snippet
    {
        public int ID { get; set; }                
        public string Code { get; set; }
        public string Output { get; set; }
        public int OrderNumber { get; set; }
        public string Question { get; set; }

        public virtual ICollection<Group> Groups { get; set; }
        public virtual ICollection<SnippetOperation> Operations { get; set; }
        public virtual ICollection<Event> Events{ get; set; }
    }
}

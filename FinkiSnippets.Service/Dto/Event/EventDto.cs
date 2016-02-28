using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinkiSnippets.Service.Dto
{
    public class EventDto
    {
        public int ID { get; set; }
        public String Name { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}

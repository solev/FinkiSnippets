using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class UserEvents
    {
        public int ID { get; set; }

        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public virtual ApplicationUser User { get; set; }

        public int EventID { get; set; }
        [ForeignKey("EventID")]
        public virtual Event Event { get; set; }

        public bool Finished { get; set; }
    }
}

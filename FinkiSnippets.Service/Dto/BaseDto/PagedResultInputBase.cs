using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinkiSnippets.Service.Dto
{
    public class PagedResultInputBase
    {
        public int page { get; set; }
        public string orderby { get; set; }
        public string option { get; set; }
        public int PageSize { get; set; }
        public string search { get; set; }
    }
}

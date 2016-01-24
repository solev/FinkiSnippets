using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinkiSnippets.Service.Dto
{
    public class ExportResultsDto
    {
        public DataTable table{ get; set; }
        public string Name { get; set; }
    }
}

using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinkiSnippets.Service.Dto
{
    public class SnippetsByGroupDto
    {
        public Group group { get; set; }
        public List<Snippet> snippets { get; set; }
    }
}

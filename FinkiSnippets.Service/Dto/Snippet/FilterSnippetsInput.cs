using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinkiSnippets.Service.Dto
{
    public class FilterSnippetsInput
    {
        public List<int> SelectedGroups { get; set; }
        public List<int> SelectedOperations { get; set; }

        //Already selected snippets, dont include in filter
        public string selectedSnippets { get; set; }
    }
}

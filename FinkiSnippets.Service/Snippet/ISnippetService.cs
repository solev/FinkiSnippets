using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinkiSnippets.Service
{
    public interface ISnippetService
    {
        bool SubmitAnswer(string UserID, int SnippetID, string Answer);
        Snippet 
    }
}

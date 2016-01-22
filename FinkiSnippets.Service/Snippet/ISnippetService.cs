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
        int GetLastAnsweredSnippetOrderNumber(string userID,int EventID,int GroupID);
        int GetLastSnippetOrderNumber(int GroupID);
        Snippet GetSnippetWithOrderNumber(int OrderNumber, int GroupID);
        bool CheckIfFirstSnippetAccess(string userID, int snippetID, int EventID);
        bool CreateInitialAnswer(AnswerLog answer);
    }
}

using Entity;
using FinkiSnippets.Service.Dto;
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
        int GetLastAnsweredSnippetOrderNumber(string userID,int EventID);
        int GetLastSnippetOrderNumber(int GroupID);
        EventSnippets GetSnippetWithOrderNumber(int OrderNumber, int EventID);
        bool CheckIfFirstSnippetAccess(string userID, int snippetID, int EventID);
        bool CreateInitialAnswer(AnswerLog answer);
        bool AddOrUpdateSnippet(Snippet snippet, List<OperatorsHelper> Operators, List<Group> GroupsString);
        List<Operation> GetAllOperations();
        List<Snippet> GetAllSnippets(int page, int snippetsPerPage);
        List<Snippet> FilterSnippets(FilterSnippetsInput input);
        List<Snippet> GetSnippetsFromGroup(int groupID);
        Snippet GetSnippetById(int snippetID);

        List<Snippet> GetAllSnippetsByID(List<Int32> IDs);
        bool DeleteSnippet(int snippetID);
    }
}

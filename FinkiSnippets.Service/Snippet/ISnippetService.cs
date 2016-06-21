using Entity;
using FinkiSnippets.Entity;
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
        bool SubmitAnswer(string UserID, int EventID, int SnippetID, string Answer);        
        int GetLastAnsweredSnippetOrderNumber(string userID,int EventID);
        EventSnippets GetSnippetWithOrderNumber(int OrderNumber, int EventID);
        bool CheckIfFirstSnippetAccess(string userID, int snippetID, int EventID);
        bool CreateInitialAnswer(string UserID, int EventID, int SnippetID);
        bool AddOrUpdateSnippet(Snippet snippet, List<OperatorsHelper> Operators, List<Group> GroupsString);
        List<Operation> GetAllOperations();
        List<Snippet> GetAllSnippets(int page, int snippetsPerPage);
        List<Snippet> FilterSnippets(FilterSnippetsInput input);
        List<Snippet> GetSnippetsFromGroup(int groupID);
        Snippet GetSnippetById(int snippetID);
        List<Snippet> GetAllSnippetsByID(List<Int32> IDs);
        bool DeleteSnippet(int snippetID);
        List<TemporarySnippet> GetAllTemporarySnippets();
        bool AddTemporarySnippets(List<TemporarySnippet> tmpSnippets);
        TemporarySnippet GetTemporarySnippetById(int tmpSnippetID);
        bool DeleteTemporarySnippetById(int tmpSnippetID);
    }
}

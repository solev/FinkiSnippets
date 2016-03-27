using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinkiSnippets.Service.Groups
{
    public interface IGroupService
    {
        List<Group> GetAllGroups();
        Group GetGroupByID(int groupID);
        int RemoveSnippetFromGroup(int SnippetID, int GroupID);
        int AddOrUpdateGroup(Group group);
        bool DeleteGroup(int GroupID);
    }
}

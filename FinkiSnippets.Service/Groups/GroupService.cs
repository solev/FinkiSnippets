using Entity;
using FinkiSnippets.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace FinkiSnippets.Service.Groups
{
    public class GroupService : IGroupService
    {
        private CodeDatabase db;

        public GroupService(CodeDatabase _db)
        {
            db = _db;
        }

        public List<Group> GetAllGroups()
        {
            var result = db.Groups.ToList();
            return result;
        }

        public Group GetGroupByID(int groupID)
        {
            var result = db.Groups.Find(groupID);
            return result;
        }

        public int RemoveSnippetFromGroup(int SnippetID, int GroupID)
        {
            Group group = db.Groups.Where(x => x.ID == GroupID).Include(x => x.Snippets).FirstOrDefault();
            Snippet snippetFromGroup = group.Snippets.FirstOrDefault(x => x.ID == SnippetID);
            group.Snippets.Remove(snippetFromGroup);
            int res = db.SaveChanges();

            return res;
        }

        public int AddOrUpdateGroup(Group group)
        {
            int res;

            if(group.ID > 0)
            {
                Group gr = db.Groups.Find(group.ID);
                gr.Name = group.Name;
                res = db.SaveChanges();
                return res;
            }

            db.Groups.Add(group);
            res = db.SaveChanges();
            return res;
        }

        public bool DeleteGroup(int GroupID)
        {
            Group g = db.Groups.Find(GroupID);
            db.Groups.Remove(g);

            int res = db.SaveChanges();

            return res > 0;
        }
    }
}

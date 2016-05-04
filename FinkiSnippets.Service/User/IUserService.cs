using Entity;
using FinkiSnippets.Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinkiSnippets.Service
{
    public interface IUserService
    {
        ListUsersDto GetAllUsers(ListUsersInput input);
        EventSnippets UserActiveEvent(string UserID);
        EventSnippets BeginEvent(string UserID, int EventID);

        ListUsersDto GetUsers(String Query);
    }
}

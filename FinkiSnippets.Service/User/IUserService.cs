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
        ListUsersDto GetAllUsers(int page, int usersPerPage);
        Event GetCurrentEvent(string UserID);
        bool BeginEvent(string UserID, int EventID);
    }
}

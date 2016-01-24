using FinkiSnippets.Data;
using FinkiSnippets.Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinkiSnippets.Service
{
    public class UserService : IUserService
    {
        private CodeDatabase db;
               
        public UserService(CodeDatabase _db)
        {
            db = _db;
        }
        
        public ListUsersDto GetAllUsers(int page, int usersPerPage)
        {

            var query = db.Users.OrderBy(x => x.Id);

            var tempResult = query.Skip((page - 1) * usersPerPage).Take(usersPerPage).Select(x => new
            {
                x.Id,
                x.FirstName,
                x.LastName,
                x.UserName
            });

            ListUsersDto result = new ListUsersDto
            {
                TotalCount = query.Count(),
                Users = tempResult.Select(x => new UserDto
                {
                    ID = x.Id,
                    FirstName = x.FirstName,
                    LastName=x.LastName,
                    Username = x.UserName
                }).ToList()
            };

            return result;
        }
    }
}

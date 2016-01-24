using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinkiSnippets.Service.Dto
{
    public class ListUsersDto
    {
        public int TotalCount { get; set; }
        public List<UserDto> Users { get; set; }
    }
}

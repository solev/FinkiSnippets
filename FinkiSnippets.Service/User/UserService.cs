using FinkiSnippets.Data;
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
        
        public void Dispose()
        {
            
        }
    }
}

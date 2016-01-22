using Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinkiSnippets.Data
{
    public class CodeDatabase : IdentityDbContext<ApplicationUser>
    {
        public CodeDatabase() : base("name = DefaultConnection") 
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
        public DbSet<Snippet> Snippets { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<AnswerLog> Answers { get; set; }
        public DbSet<SnippetOperation> SnippetOperations { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Group> Groups{ get; set; }   

    }
}

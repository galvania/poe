using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poe_stash_crawler
{
    public class CFContext : DbContext
    {
        public CFContext() : base("name=poe_dbConnnectionString")
        {

        }
        public DbSet<Stash> Stashes { get; set; }

    }
}

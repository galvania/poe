using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poe_stash_crawler
{
    [Serializable]
    public class Sockets
    {
        public int ID { get; set; }

        public int group { get; set; }
        public string attr { get; set; }
    }
}

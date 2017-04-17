using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poe_stash_crawler
{
    public class Properties
    {
        public int ID { get; set; }
        public string name { get; set; }
        public dynamic[] values { get; set; }
        public int displayMode { get; set; }
        public int type{ get; set; }
        public int progress { get; set; }
    }
}

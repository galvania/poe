using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poe_stash_crawler
{
    [Serializable]
    public class Item
    {
       

        public bool verified { get; set; }
        public int w { get; set; }
        public int h { get; set; }
        public int ilvl { get; set; }
        public string icon { get; set; }
        public string league { get; set; }
        public string id { get; set; }
        //sockets,See,below,array,of,sockets,array[assoc]
        public List<Sockets> sockets { get; set; }
        public string name { get; set; }
        public string typeLine { get; set; }
        public bool identified { get; set; }
        public bool corrupted { get; set; }
        public bool lockedToCharacter { get; set; }
        public string note { get; set; }
        //properties,See,below,array[assoc]
        public List<Properties> properties { get; set; }

        //requirements,See,below,array[assoc]
        public List<Requirements> requirements { get; set; }

        //explicitMods,string,array
        public List<string> explicitMods { get; set; }

        //implicitMods,string,array
        public List<string> implicitMods { get; set; }

        //enchantMods,labyrinth,mods,string,array
        public List<string> enchantMods { get; set; }

        //craftedMods,master,mods,string,array
        public List<string> craftedMods { get; set; }

        //flavourText,string,array
        public List<string> flavourText { get; set; }

        public int frameType { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public string inventoryId { get; set; }
        //socketedItems,See,items,array[assoc]
        public List<Item> socketedItems { get; set; }

        //additionalProperties,See,properties,array[assoc]
        public List<Properties> additionalProperties { get; set; }

        public string secDescrText { get; set; }
        public string descrText { get; set; }
        public string artFilename { get; set; }
        public bool duplicated { get; set; }
        public int maxStackSize { get; set; }
        //nextLevelRequirements,See,requirements,array[assoc]
        public List<Requirements> nextLevelRequirements { get; set; }

        public int stackSize { get; set; }
        public int talismanTier { get; set; }
        //utilityMods,flask,utility,mods,string,array
        public List<string> utilityMods { get; set; }

        public bool support { get; set; }
        //cosmeticMods,string,array
        public List<string> cosmeticMods { get; set; }

        public string prophecyDiffText { get; set; }
        public string prophecyText { get; set; }
        public bool isRelic { get; set; }
    }
}

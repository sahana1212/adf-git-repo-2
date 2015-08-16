using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visual_Novel_Database
{
    public class CharacterItem
    {
        public int id { get; set; }
        public string description { get; set; }
        public string aliases { get; set; }
        public string image { get; set; }
    }

    public class CharacterRootObject
    {
        public List<CharacterItem> items { get; set; }
        public bool more { get; set; }
        public int num { get; set; }
    }
}

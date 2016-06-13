using System.Collections.Generic;

namespace Visual_Novel_Manager.JSON
{
    

    public class CharacterRootObject
    {
        public List<CharacterItem> items { get; set; }
        public bool more { get; set; }
        public int num { get; set; }
    }

    public class CharacterItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public string original { get; set; }
        public string gender { get; set; }
        public string bloodt { get; set; }
        public List<int?> birthday { get; set; }
        public object aliases { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public object bust { get; set; }
        public object waist { get; set; }
        public object hip { get; set; }
        public object height { get; set; }
        public object weight { get; set; }
        public List<List<int>> traits { get; set; }
        public List<List<object>> vns { get; set; }
    }





}

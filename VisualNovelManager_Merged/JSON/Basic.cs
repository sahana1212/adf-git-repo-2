using System.Collections.Generic;

namespace VisualNovelManager.JSON
{
    //These classes contain the information
    //received from a 'get vn basic' command
    public class BasicItem
    {
        public List<string> platforms { get; set; }
        public int id { get; set; }
        public string released { get; set; }
        public string original { get; set; }
        public List<string> orig_lang { get; set; }
        public string title { get; set; }
        public List<string> languages { get; set; }              
    }

    public class BasicRootObject
    {
        public List<BasicItem> items { get; set; }
        public bool more { get; set; }
        public int num { get; set; }
    }   
}

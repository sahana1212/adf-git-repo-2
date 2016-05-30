using System.Collections.Generic;

namespace VisualNovelManager.JSON
{
    //These classes contain the information
    //received from a 'get vn tags' command
    public class TagsItem
    {
        public List<List<double>> tags { get; set; }
        public int id { get; set; }
    }

    public class TagsRootObject
    {
        public List<TagsItem> items { get; set; }
        public bool more { get; set; }
        public int num { get; set; }
    }
}

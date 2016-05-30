using System.Collections.Generic;

namespace VisualNovelManager.JSON
{
    //used to read tag dump of traits
    public class WrittenTraitsRootObject
    {
        public string name { get; set; }
        public string description { get; set; }
        public int id { get; set; }
        public List<object> parents { get; set; }
        public bool meta { get; set; }
        public int chars { get; set; }
        public List<object> aliases { get; set; }
    }
}

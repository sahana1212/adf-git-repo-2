using System.Collections.Generic;

namespace Visual_Novel_Manager.JSON
{
    //These classes contain the information
    //received from a 'get vn tags' command
    public class TraitsItem
    {
        public List<List<double>> traits { get; set; }
        public int id { get; set; }
    }

    public class TraitsRootObject
    {
        public List<TraitsItem> items { get; set; }
        public bool more { get; set; }
        public int num { get; set; }
    }
}

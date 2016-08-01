using System.Collections.Generic;

namespace Visual_Novel_Manager.JSON
{
    public class StatsItem
    {
        public double popularity { get; set; }
        public int votecount { get; set; }
        public double rating { get; set; }
        public int id { get; set; }
    }

    public class StatsRootObject
    {
        public bool more { get; set; }
        public int num { get; set; }
        public List<StatsItem> items { get; set; }
    }
}

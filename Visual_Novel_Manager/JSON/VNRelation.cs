using System.Collections.Generic;

namespace Visual_Novel_Manager.JSON
{
    public class Relation
    {
        public int id { get; set; }
        public string title { get; set; }
        public string relation { get; set; }
        public string original { get; set; }
    }

    public class VNRelationItem
    {
        public List<Relation> relations { get; set; }
        public int id { get; set; }
    }

    public class VnRelationRootObject
    {
        public bool more { get; set; }
        public int num { get; set; }
        public List<VNRelationItem> items { get; set; }
    }
}

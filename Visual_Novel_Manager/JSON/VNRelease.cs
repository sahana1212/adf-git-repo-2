using System.Collections.Generic;

namespace Visual_Novel_Manager.JSON
{
    public class VnReleaseRootObject
    {
        public List<ReleaseItem> items { get; set; }
        public bool more { get; set; }
        public int num { get; set; }
    }


    public class ReleaseItem
    {
        public bool doujin { get; set; }
        public int id { get; set; }
        public string type { get; set; }
        public bool patch { get; set; }
        public string released { get; set; }
        public bool freeware { get; set; }
        public string original { get; set; }
        public string title { get; set; }
        public List<string> languages { get; set; }
        public string gtin { get; set; }
        public int? minage { get; set; }
        public List<string> platforms { get; set; }
        public List<object> media { get; set; }
        public string catalog { get; set; }
        public string website { get; set; }
        public string notes { get; set; }
        public List<ReleaseVn> vn { get; set; }
        public List<Producer> producers { get; set; }
    }
    public class ReleaseVn
    {
        public string original { get; set; }
        public string title { get; set; }
        public int id { get; set; }
    }
    public class Producer
    {
        public int id { get; set; }
        public string type { get; set; }
        public bool developer { get; set; }
        public string name { get; set; }
        public bool publisher { get; set; }
        public string original { get; set; }
    }










    public class ReleaseMedia
    {
        public string medium { get; set; }
        public int? qty { get; set; }
    }
}

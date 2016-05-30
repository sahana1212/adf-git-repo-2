using System.Collections.Generic;

namespace VisualNovelManager.JSON
{
    //These classes contain the information
    //received from a 'get vn detail' command
    public class DetailsLinks
    {
        public object renai { get; set; }
        public object encubed { get; set; }
        public object wikipedia { get; set; }
    }

    public class DetailsItem
    {
        public DetailsLinks links { get; set; }
        public string image { get; set; }
        public bool image_nsfw { get; set; }
        public object aliases { get; set; }
        public string description { get; set; }
        public int id { get; set; }
        public object length { get; set; }
    }

    public class DetailsRootObject
    {
        public int num { get; set; }
        public List<DetailsItem> items { get; set; }
        public bool more { get; set; }
    }
}

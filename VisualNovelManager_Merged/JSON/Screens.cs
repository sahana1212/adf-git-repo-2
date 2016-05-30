using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualNovelManager.JSON
{
    public class Screen
    {
        public int rid { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string image { get; set; }
        public bool nsfw { get; set; }
    }

    public class ScreenItem
    {
        public int id { get; set; }
        public List<Screen> screens { get; set; }
    }

    public class ScreenRootObject
    {
        public List<ScreenItem> items { get; set; }
        public int num { get; set; }
        public bool more { get; set; }
    }
}

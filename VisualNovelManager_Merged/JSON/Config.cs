using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualNovelManager.JSON
{
    public class Global
    {
        public bool NsfwEnabled { get; set; }
        public int VnSpoilerLevel { get; set; }
        public int CharacterSpoilerLevel { get; set; }
    }

    public class Unique
    {
        public int VnId { get; set; }
        public int VnSpoilerLevel { get; set; }
        public int CharacterSpoilerLevel { get; set; }
    }

    public class ConfigRootObject
    {
        public Global global { get; set; }
        public List<Unique> unique { get; set; }
    }
}

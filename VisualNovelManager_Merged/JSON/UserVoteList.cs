using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualNovelManager.JSON
{
    public class UserVoteList
    {
        public int vn { get; set; }
        public int added { get; set; }
        public int vote { get; set; }
    }

    public class VoteListRootObject
    {
        public bool more { get; set; }
        public int num { get; set; }
        public List<UserVoteList> items { get; set; }
    }
}

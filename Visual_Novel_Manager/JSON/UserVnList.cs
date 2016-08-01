using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visual_Novel_Manager.JSON
{
    public class UserVnList
    {
        public int added { get; set; }
        public object notes { get; set; }
        public int status { get; set; }
        public int vn { get; set; }
    }

    public class UserVnListRootObject
    {
        public bool more { get; set; }
        public int num { get; set; }
        public List<UserVnList> items { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visual_Novel_Manager.JSON
{
    public class ErrorRootObject
    {
        
        public string type { get; set; }
        public double minwait { get; set; }
        public double fullwait { get; set; }
        public string msg { get; set; }
        public string id { get; set; }


        public int value { get; set; }
        public string op { get; set; }
        public string field { get; set; }
    }
}

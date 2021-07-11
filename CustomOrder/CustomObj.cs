using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomOrder
{
    enum CustomState 
    {
        ready, going, done, deny, wating, price
    }
    class CustomObj
    {
        public long qq { get; set; }
        public string text { get; set; }
        public string id { get; set; }
        public int cost { get; set; }
        public CustomState state { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomOrder
{
    record RobotObj
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public long RunQQ { get; set; }
        public int Time { get; set; }
        public bool Check { get; set; }
    }
    record TextObj 
    {
        public string Help { get; set; }
        public string Start { get; set; }
        public string Start1 { get; set; }
        public string Cost { get; set; }
        public string Start2 { get; set; }
        public string My { get; set; }
        public string Now { get; set; }
        public string Last { get; set; }
        public string NoText { get; set; }
        public string NoLast { get; set; }
    }
    record CommandObj 
    {
        public string Head { get; set; }
        public string Start { get; set; }
        public string Text { get; set; }
        public string Help { get; set; }
        public string My { get; set; }
        public string Now { get; set; }
        public string Confirm { get; set; }
        public string Refuse { get; set; }
    }
    record ConfigObj
    {
        public RobotObj Robot { get; set; }
        public string NowCustom { get; set; }
        public long Admin { get; set; }
        public TextObj Text { get; set; }
        public CommandObj Command { get; set; }
    }
}

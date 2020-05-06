using System.Collections.Generic;

namespace RDB.Interface.RDBObjects
{
    public class RdbComponent
    {
        public string name { get; set; }
        public IEnumerable<Pin> pins { get; set; }
    }
}
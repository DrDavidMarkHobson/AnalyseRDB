using System.Collections.Generic;

namespace RDB.Interface.RDBObjects
{
    public class RdbNet
    {
        public string name { get; set; }
        public Prop prop { get; set; }
        public IEnumerable<Pin> pins { get; set; }
        public IEnumerable<RdbComponent> components { get; set; }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace RDB.Interface.RDBObjects
{
    public class RdbNets
    {
        private float _cenX;
        private float _cenY;
        public string fileName { get; set; }
        public IEnumerable<RdbNet> Nets { get; set; }
        public float CentroidX => _cenX;
        public float CentroidY => _cenY;

        public void UpdateCentroid()
        {
            var pins = Nets.SelectMany(net => net.pins).Where(pin => pin.name != "_");
            _cenX = pins.Sum(pin => pin.x) / pins.Count();
            _cenY = pins.Sum(pin => pin.y) / pins.Count();
        }
    }
}
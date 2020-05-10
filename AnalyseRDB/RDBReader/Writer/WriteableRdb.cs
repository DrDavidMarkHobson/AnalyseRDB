using System.Collections.Generic;
using RDB.Interface.RDBObjects;

namespace RDBData.Writer
{
    public class WriteableRdb : IWriteableRdb{
        public string[] Write(RdbNets data)
        {
            var lines = new List<string>();

            foreach (var dataNet in data.Nets)
            {
                lines.Add(string.Format(RdbFileLines.NetLine, dataNet.name));
                lines.Add(string.Format(RdbFileLines.PropLine, dataNet.prop));
                foreach (var pin in dataNet.pins)
                {
                    lines.Add(string.Format(RdbFileLines.PinLine, pin.name, pin.x, pin.y));
                }
            }

            return lines.ToArray();
        }
    }
}
using System.Drawing;
using RDB.Interface.RDBObjects;

namespace RDBData.Render
{
    public interface IRenderRdb
    {
        Image Convert(RdbNets data);
    }
}
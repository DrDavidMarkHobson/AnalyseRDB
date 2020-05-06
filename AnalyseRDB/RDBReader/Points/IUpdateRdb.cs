using RDB.Interface.RDBObjects;

namespace RDBData.Points
{
    public interface IUpdateRdb
    {
        RdbNets RotateAround(RdbNets nets, Point pivot, float angle);
    }
}
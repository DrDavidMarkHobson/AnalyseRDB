using RDB.Interface.RDBObjects;

namespace RDBData.Points
{
    public interface IMovePoint
    {
        Point RotateAround(Point point, Point pivot, float angle);
    }
}
using System;
using System.Linq;
using RDB.Interface.RDBObjects;

namespace RDBData.Points
{
    public class MovePoint : IMovePoint
    {
        public Point RotateAround(Point point, Point pivot, float angle)
        {
            var angleInRadians = angle * (float)(Math.PI / 180);
            var cosTheta = (float)Math.Cos(angleInRadians);
            var sinTheta = (float)Math.Sin(angleInRadians);

            return new Point
            {
                X = cosTheta * (point.X - pivot.X) -
                    sinTheta * (point.Y - pivot.Y) + pivot.X,
                Y = sinTheta * (point.X - pivot.X) +
                    cosTheta * (point.Y - pivot.Y) + pivot.Y
            };
        }
    }
    public static class ManipulatePoints
    {
        public static Point RotateAround(Point point, Point pivot, float angle)
        {
            var angleInRadians = angle * (float)(Math.PI / 180);
            var cosTheta = (float)Math.Cos(angleInRadians);
            var sinTheta = (float)Math.Sin(angleInRadians);

            return new Point
            {
                X = cosTheta * (point.X - pivot.X) -
                    sinTheta * (point.Y - pivot.Y) + pivot.X,
                Y = sinTheta * (point.X - pivot.X) +
                    cosTheta * (point.Y - pivot.Y) + pivot.Y
            };
        }
    }
}
using System.Collections.Generic;
using RDB.Interface.RDBObjects;

namespace RDBData.Points
{
    public class UpdateRDB : IUpdateRdb
    {
        private readonly IMovePoint _movePoint;

        public UpdateRDB() : this(new MovePoint()) { }

        public UpdateRDB(IMovePoint movePoint)
        {
            _movePoint = movePoint;
        }

        public RdbNets RotateAround(RdbNets nets, Point pivot, float angle)
        {
            foreach (var net in nets.Nets)
            {
                var newPins = new List<Pin>();
                foreach (var pin in net.pins)
                {
                    var point = new Point
                    {
                        X = pin.x,
                        Y = pin.y
                    };
                    if (pin.name != "_")
                    {
                        point =
                            _movePoint.RotateAround(point, pivot, angle);
                    }

                    newPins.Add(new Pin
                    {
                        name = pin.name,
                        x = point.X,
                        y = point.Y
                    });
                }
                net.pins = newPins;
            }

            nets.UpdateCentroid();

            return nets;
        }
    }
}
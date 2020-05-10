using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RDB.Interface.RDBObjects;

namespace RDBData.Points
{
    public class UpdateRDB : IUpdateRdb
    {
        private readonly IMovePoint _movePoint;
        public int Index { get; private set; }
        public int NumberOfEntries { get; private set; }

        public BackgroundWorker BackgroundWorker { get; set; }

        public UpdateRDB() : this(new MovePoint()) { }


        public UpdateRDB(IMovePoint movePoint)
        {
            _movePoint = movePoint;
            NumberOfEntries = 0;
            Index = 0;
        }

        public async Task<RdbNets> RotateAround(RdbNets nets, Point pivot, float angle)
        {
            NumberOfEntries = nets.Nets.SelectMany(net => net.pins).Count();
            Index = 0;
            int oldPercent = 0;
            int newPercent = 0;

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
                    Index++;

                    oldPercent = newPercent;
                    newPercent = (Index * 100) / NumberOfEntries;
                    BackgroundWorker.ReportProgress(newPercent, (int)1);
                    if (newPercent > oldPercent)
                    {
                        Thread.Sleep(1);
                    }
                }
                net.pins = newPins; 
            }

            BackgroundWorker.ReportProgress(newPercent);
            if (newPercent > oldPercent)
            {
                Thread.Sleep(1);
            }

            nets.UpdateCentroid();

            BackgroundWorker.ReportProgress(100);
            Thread.Sleep(1);

            return nets;
        }
    }
}
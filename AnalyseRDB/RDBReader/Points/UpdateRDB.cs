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

        public UpdateRDB(
            RdbNets taskNets,
            Point pivot,
            float angle
            )
        {
            _taskNets = taskNets;
            _pivot = pivot;
            _angle = angle;
            _newPins = new List<Pin>();
            _intervals = 100;
            _currentIndex = 0;
            _taskDone = false;
        }

        private RdbNets _taskNets;
        private Point _pivot;
        private float _angle;
        private int _intervals;
        private int _currentIndex;
        private Pin[] _pins => _taskNets.Nets.SelectMany(net => net.pins).ToArray();
        private List<Pin> _newPins;
        private int _intervalSpan => _pins.Length / _intervals;
        private bool _taskDone;
        public bool TaskDone => _taskDone;

        public async Task<int> RotateAround()
        {
            Thread.Sleep(1);
            if (_currentIndex < 0)
            {
                _currentIndex = 0;
            }
            int nextInterval = _currentIndex + _intervalSpan;
            if (nextInterval > _pins.Length)
            {
                nextInterval = _pins.Length;
            }
            while(_currentIndex < nextInterval)
            {
                RotatePin(_currentIndex++);
            }
            if (_currentIndex == _pins.Length)
            {
                _taskDone = true;
            }
            var result = _currentIndex / _intervalSpan;
            return result;
        }

        private void RotatePin(int _currentIndex)
        {
            var point = new Point
            {
                X = _pins[_currentIndex].x,
                Y = _pins[_currentIndex].y
            };
            if (_pins[_currentIndex].name != "_")
            {
                point = ManipulatePoints.RotateAround(point, _pivot, _angle);
            }
            _newPins.Add(new Pin
            {
                name = _pins[_currentIndex].name,
                x = point.X,
                y = point.Y
            });
        }

        public async Task<RdbNets> RotateAround(
            RdbNets nets, 
            Point pivot, 
            float angle, 
            BackgroundWorker bgw)
        {
            BackgroundWorker = bgw;
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
                        Thread.Sleep(5);
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
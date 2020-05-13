using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using RDB.Interface.RDBObjects;
using Point = RDB.Interface.RDBObjects.Point;

namespace RDBData.Render
{
    public class ThreadedWorker
    {
    }

    public class RenderRdb : ThreadedWorker, IRenderRdb
    {
        private Bitmap _image; 
        public BackgroundWorker BackgroundWorker { get; set; }

        public RenderRdb()
        {
            BackgroundWorker = null;
            _image = new Bitmap(800, 600);
        }

        public Image Convert(RdbNets data)
        {
            int oldPercent = 0;
            int newPercent = 0;
            var points = data.Nets.SelectMany(net =>
                net.pins.Select(pin =>
                    new Point {X = pin.x, Y = pin.y}));

            foreach (var point in points)
            {
                if (
                    point.X >= 0 && 
                    point.Y >= 0 && 
                    point.X < _image.Width && 
                    point.Y < _image.Height)
                {
                    _image.SetPixel(
                        (int) point.X, (int) point.Y, Color.White);

                    //BackgroundWorker.ReportProgress(newPercent, (int)2);
                    if (newPercent > oldPercent)
                    {
                        Thread.Sleep(1);
                    }

                }
            }

            //BackgroundWorker.ReportProgress(100, (int)2);
            //Thread.Sleep(1);

            return _image;
        }
    }
}
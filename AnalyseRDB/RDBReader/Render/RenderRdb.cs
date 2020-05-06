using System.Drawing;
using System.Linq;
using RDB.Interface.RDBObjects;
using Point = RDB.Interface.RDBObjects.Point;

namespace RDBData.Render
{
    public class RenderRdb : IRenderRdb
    {
        private Bitmap _image;

        public RenderRdb()
        {
            _image = new Bitmap(800, 600);
        }

        public Image Convert(RdbNets data)
        {
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
                }
            }

            return _image;
        }
    }
}
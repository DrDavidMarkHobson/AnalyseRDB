using System;
using System.Collections.Generic;
using System.Drawing;
using RDB.Interface.RDBObjects;
using System.Linq;
using Point = RDB.Interface.RDBObjects.Point;

namespace RDBData.Reader
{
    public interface IRenderRdb
    {
        Image Convert(RdbNets data);
    }

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
                if (point.X >= 0 && point.Y >= 0 && point.X < _image.Width && point.Y < _image.Height)
                {
                    _image.SetPixel(
                        (int) point.X, (int) point.Y, Color.White);
                }
            }

            return _image;
        }
    }

    public class ReadRdb : IReadRdb
    {
        private readonly IOpenAndReadRDBFile _fileReader;

        public ReadRdb(IOpenAndReadRDBFile fileReader)
        {
            _fileReader = fileReader;
        }

        public IEnumerable<RdbNet> Read(string filePath)
        {
            var file = _fileReader.ReadAll(filePath);
            var rdbNets = new List<RdbNet>();
            var current = new RdbNet();
            var currentPins = new List<Pin>();
            var index = 0;
            
            while(index < file.Length)
            {
                var line = file[index];

                if (line.StartsWith(RdbFileLines.Net))
                {
                    current.pins = currentPins;
                    if (current.name != null)
                    {
                        rdbNets.Add(current);
                    }
                    currentPins = new List<Pin>();
                    current = new RdbNet
                    {
                        name = line.Substring(RdbFileLines.Net.Length + 1)
                    };
                }

                if (line.StartsWith(RdbFileLines.Prop))
                {
                    var prop = new Prop
                    {
                        type = line.Substring(RdbFileLines.Prop.Length+1)
                    };
                    current.prop = prop;
                }

                if (line.StartsWith(RdbFileLines.Pin))
                {
                    var pinLine = line.Substring(RdbFileLines.Pin.Length + 1)
                        .Split(' ');

                    var pin = new Pin
                    {
                        name = pinLine[0], 
                        x = Convert.ToSingle(pinLine[1]), 
                        y = Convert.ToSingle(pinLine[2])
                    };

                    currentPins.Add(pin);
                }
                index++;
            }

            current.pins = currentPins;
            if (current.name != null)
            {
                rdbNets.Add(current);
            }

            return rdbNets;
        }
    }
}
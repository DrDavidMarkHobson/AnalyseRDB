using System;
using System.Collections.Generic;
using System.IO;
using RDB.Interface.RDBObjects;

namespace RDBData.Reader
{
    public interface ISaveRdbNetsToFile
    {
        void Write(RdbNets data);
    }
    public class SaveRdbNetsToFile : ISaveRdbNetsToFile
    {
        private readonly IWriteRDBDataToFile _writeRdbDataToFile;
        private readonly IWriteableRdb _writeableRdb;

        public SaveRdbNetsToFile() : this(new WriteRDBDataToFile(), new WriteableRdb()) { }
        public SaveRdbNetsToFile(IWriteRDBDataToFile writeRdbDataToFile,
            IWriteableRdb writeableRdb)
        {
            _writeRdbDataToFile = writeRdbDataToFile;
            _writeableRdb = writeableRdb;
        }

        public void Write(RdbNets data)
        {
            _writeRdbDataToFile.Write(_writeableRdb.Write(data), data.fileName);
        }
    }

    public interface IWriteRDBDataToFile
    {
        void Write(string[] data, string fileName);
    }
    public class WriteRDBDataToFile : IWriteRDBDataToFile
    {
        public void Write(string[] data, string fileName)
        {
            File.WriteAllLines(fileName,data);
        }
    }
    public interface IWriteableRdb
    {
        string[] Write(RdbNets data);
    }

    public class WriteableRdb : IWriteableRdb{
        public string[] Write(RdbNets data)
        {
            var lines = new List<string>();

            foreach (var dataNet in data.Nets)
            {
                lines.Add(string.Format(RdbFileLines.NetLine, dataNet.name));
                lines.Add(string.Format(RdbFileLines.PropLine, dataNet.prop));
                foreach (var pin in dataNet.pins)
                {
                    lines.Add(string.Format(RdbFileLines.PinLine, pin.name, pin.x, pin.y));
                }
            }

            return lines.ToArray();
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
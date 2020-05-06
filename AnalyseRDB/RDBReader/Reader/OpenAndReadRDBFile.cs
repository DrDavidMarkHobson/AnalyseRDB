using System.IO;

namespace RDBData.Reader
{
    public class OpenAndReadRDBFile : IOpenAndReadRDBFile
    {
        public string[] ReadAll(string filePath)
        {
            var data = File.ReadAllLines(filePath);
            return data;
        }
    }
}
using System.IO;

namespace RDBData.Writer
{
    public class WriteRDBDataToFile : IWriteRDBDataToFile
    {
        public void Write(string[] data, string fileName)
        {
            File.WriteAllLines(fileName,data);
        }
    }
}
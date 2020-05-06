namespace RDBData.Writer
{
    public interface IWriteRDBDataToFile
    {
        void Write(string[] data, string fileName);
    }
}
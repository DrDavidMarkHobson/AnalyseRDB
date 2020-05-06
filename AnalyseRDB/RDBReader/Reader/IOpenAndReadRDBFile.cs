namespace RDBData.Reader
{
    public interface IOpenAndReadRDBFile
    {
        string[] ReadAll(string filePath);
    }
}
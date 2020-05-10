using RDB.Interface.RDBObjects;

namespace RDBData.Writer
{
    public interface IWriteableRdb
    {
        string[] Write(RdbNets data);
    }
}
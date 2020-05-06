using RDB.Interface.RDBObjects;

namespace RDBData.Writer
{
    public interface ISaveRdbNetsToFile
    {
        void Write(RdbNets data);
    }
}
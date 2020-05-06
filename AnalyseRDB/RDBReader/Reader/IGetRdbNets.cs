using RDB.Interface.RDBObjects;

namespace RDBData.Reader
{
    public interface IGetRdbNets
    {

        RdbNets Get(string fileName);
    }
}
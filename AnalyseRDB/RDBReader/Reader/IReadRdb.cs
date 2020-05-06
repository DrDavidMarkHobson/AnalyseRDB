using System.Collections.Generic;
using RDB.Interface.RDBObjects;

namespace RDBData.Reader
{
    public interface IReadRdb
    {
        IEnumerable<RdbNet> Read(string filePath);
    }
}
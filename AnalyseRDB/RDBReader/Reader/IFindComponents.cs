using System.Collections.Generic;
using RDB.Interface.RDBObjects;

namespace RDBData.Reader
{
    public interface IFindComponents
    {
        IEnumerable<RdbComponent> Find(IEnumerable<RdbNet> nets);
    }
}
using System.ComponentModel;
using System.Threading.Tasks;
using RDB.Interface.RDBObjects;

namespace RDBData.Points
{
    public interface IUpdateRdb
    {
        Task<RdbNets> RotateAround(RdbNets nets, Point pivot, float angle, BackgroundWorker bgw);
    }
}
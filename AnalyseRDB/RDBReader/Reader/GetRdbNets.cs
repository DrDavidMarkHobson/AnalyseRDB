using RDB.Interface.RDBObjects;

namespace RDBData.Reader
{
    public class GetRdbNets : IGetRdbNets
    {
        private readonly IReadRdb _readRdb;

        public GetRdbNets(IReadRdb readRdb)
        {
            _readRdb = readRdb;
        }
        public RdbNets Get(string fileName)
        {
            var nets = _readRdb.Read(fileName);
            var rdbNets = new RdbNets
            {
                fileName = fileName,
                Nets = nets
            };
            rdbNets.UpdateCentroid();

            return rdbNets;
        }
    }
}
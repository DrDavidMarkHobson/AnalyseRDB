using RDB.Interface.RDBObjects;
using RDBData.Reader;

namespace RDBData.Writer
{
    public class SaveRdbNetsToFile : ISaveRdbNetsToFile
    {
        private readonly IWriteRDBDataToFile _writeRdbDataToFile;
        private readonly IWriteableRdb _writeableRdb;

        public SaveRdbNetsToFile() : this(new WriteRDBDataToFile(), new WriteableRdb()) { }
        public SaveRdbNetsToFile(IWriteRDBDataToFile writeRdbDataToFile,
            IWriteableRdb writeableRdb)
        {
            _writeRdbDataToFile = writeRdbDataToFile;
            _writeableRdb = writeableRdb;
        }

        public void Write(RdbNets data)
        {
            _writeRdbDataToFile.Write(_writeableRdb.Write(data), data.fileName);
        }
    }
}
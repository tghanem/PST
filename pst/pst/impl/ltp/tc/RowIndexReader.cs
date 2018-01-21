using pst.core;
using pst.encodables.ltp.tc;
using pst.encodables.ndb;
using pst.interfaces.ltp.bth;
using pst.interfaces.ltp.hn;
using pst.interfaces.ltp.tc;
using System.Linq;

namespace pst.impl.ltp.tc
{
    class RowIndexReader : IRowIndexReader
    {
        private readonly IHeapOnNodeReader heapOnNodeReader;
        private readonly IBTreeOnHeapReader<int> bthReader;

        public RowIndexReader(
            IHeapOnNodeReader heapOnNodeReader,
            IBTreeOnHeapReader<int> bthReader)
        {
            this.bthReader = bthReader;
            this.heapOnNodeReader = heapOnNodeReader;
        }

        public Maybe<int> GetRowIndex(NID[] nodePath, int rowId)
        {
            var hnHeader = heapOnNodeReader.GetHeapOnNodeHeader(nodePath);

            var heapItem = heapOnNodeReader.GetHeapItem(nodePath, hnHeader.UserRoot);

            var tcinfo = TCINFO.OfValue(heapItem);

            var dataRecord = bthReader.ReadDataRecord(nodePath, tcinfo.RowIndex, rowId);

            if (dataRecord.HasNoValue)
            {
                return Maybe<int>.NoValue();
            }

            return dataRecord.Value.Data.ToInt32();
        }

        public TCROWID[] GetAllRowIds(NID[] nodePath)
        {
            var hnHeader = heapOnNodeReader.GetHeapOnNodeHeader(nodePath);

            var heapItem = heapOnNodeReader.GetHeapItem(nodePath, hnHeader.UserRoot);

            var tcinfo = TCINFO.OfValue(heapItem);

            return
                bthReader
                .ReadAllDataRecords(nodePath, tcinfo.RowIndex)
                .Select(r => new TCROWID(r.Key, r.Data.ToInt32()))
                .ToArray();
        }
    }
}

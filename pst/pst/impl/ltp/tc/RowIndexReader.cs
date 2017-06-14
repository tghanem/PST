using pst.core;
using pst.encodables.ltp.bth;
using pst.encodables.ltp.tc;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ltp.bth;
using pst.interfaces.ltp.hn;
using pst.interfaces.ltp.tc;
using System;
using System.Linq;

namespace pst.impl.ltp.tc
{
    class RowIndexReader<TRowId> : IRowIndexReader<TRowId> where TRowId : IComparable<TRowId>
    {
        private readonly IDecoder<TCINFO> tcinfoDecoder;
        private readonly IHeapOnNodeReader heapOnNodeReader;
        private readonly IBTreeOnHeapReader<TRowId> bthReader;
        private readonly IConverter<DataRecord, TCROWID> dataRecordToTCROWIDConverter;

        public RowIndexReader(
            IDecoder<TCINFO> tcinfoDecoder,
            IHeapOnNodeReader heapOnNodeReader,
            IBTreeOnHeapReader<TRowId> bthReader,
            IConverter<DataRecord, TCROWID> dataRecordToTCROWIDConverter)
        {
            this.bthReader = bthReader;
            this.tcinfoDecoder = tcinfoDecoder;
            this.heapOnNodeReader = heapOnNodeReader;
            this.dataRecordToTCROWIDConverter = dataRecordToTCROWIDConverter;
        }

        public Maybe<TCROWID> GetRowId(BID blockId, TRowId rowId)
        {
            var hnHeader =
                heapOnNodeReader.GetHeapOnNodeHeader(blockId);

            var heapItem =
                heapOnNodeReader.GetHeapItem(blockId, hnHeader.UserRoot);

            var tcinfo =
                tcinfoDecoder.Decode(heapItem);

            var tcRowId =
                bthReader.ReadDataRecord(blockId, tcinfo.RowIndex, rowId);

            if (tcRowId.HasNoValue)
                return Maybe<TCROWID>.NoValue();

            return dataRecordToTCROWIDConverter.Convert(tcRowId.Value);
        }

        public TCROWID[] GetAllRowIds(BID blockId)
        {
            var hnHeader =
                heapOnNodeReader.GetHeapOnNodeHeader(blockId);

            var heapItem =
                heapOnNodeReader.GetHeapItem(blockId, hnHeader.UserRoot);

            var tcinfo =
                tcinfoDecoder.Decode(heapItem);

            return
                bthReader
                .ReadAllDataRecords(blockId, tcinfo.RowIndex)
                .Select(dataRecordToTCROWIDConverter.Convert)
                .ToArray();
        }
    }
}

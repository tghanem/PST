using pst.core;
using pst.encodables.ltp.bth;
using pst.encodables.ltp.tc;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.io;
using pst.interfaces.ltp.bth;
using pst.interfaces.ltp.hn;
using pst.interfaces.ltp.tc;
using System;
using System.Linq;

namespace pst.impl.ltp.tc
{
    class RowIndexReader<TRowId>
        : IRowIndexReader<TRowId> where TRowId : IComparable<TRowId>
    {
        private readonly IConverter<DataRecord, TCROWID> dataRecordToTCROWIDConverter;
        private readonly IBTreeOnHeapReader<TRowId> bthReader;
        private readonly IHeapOnNodeReader heapOnNodeReader;
        private readonly IDecoder<TCINFO> tcinfoDecoder;

        public RowIndexReader(
            IConverter<DataRecord, TCROWID> dataRecordToTCROWIDConverter,
            IBTreeOnHeapReader<TRowId> bthReader,
            IHeapOnNodeReader heapOnNodeReader,
            IDecoder<TCINFO> tcinfoDecoder)
        {
            this.dataRecordToTCROWIDConverter = dataRecordToTCROWIDConverter;
            this.bthReader = bthReader;
            this.heapOnNodeReader = heapOnNodeReader;
            this.tcinfoDecoder = tcinfoDecoder;
        }

        public Maybe<TCROWID> GetRowId(
            IDataBlockReader<LBBTEntry> reader,
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry,
            TRowId rowId)
        {
            var hnHeader =
                heapOnNodeReader
                .GetHeapOnNodeHeader(reader, blockIdToEntryMapping, blockEntry);

            var tcinfo =
                tcinfoDecoder
                .Decode(
                    heapOnNodeReader
                    .GetHeapItem(reader, blockIdToEntryMapping, blockEntry, hnHeader.UserRoot));

            var tcRowId =
                bthReader.ReadDataRecord(reader, blockIdToEntryMapping, blockEntry, tcinfo.RowIndex, rowId);

            if (tcRowId.HasNoValue)
                return Maybe<TCROWID>.NoValue();

            return dataRecordToTCROWIDConverter.Convert(tcRowId.Value);
        }

        public TCROWID[] GetAllRowIds(
            IDataBlockReader<LBBTEntry> reader,
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry)
        {
            var hnHeader =
                heapOnNodeReader
                .GetHeapOnNodeHeader(reader, blockIdToEntryMapping, blockEntry);

            var tcinfo =
                tcinfoDecoder
                .Decode(
                    heapOnNodeReader
                    .GetHeapItem(reader, blockIdToEntryMapping, blockEntry, hnHeader.UserRoot));

            return
                bthReader
                .ReadAllDataRecords(reader, blockIdToEntryMapping, blockEntry, tcinfo.RowIndex)
                .Select(dataRecordToTCROWIDConverter.Convert)
                .ToArray();
        }
    }
}

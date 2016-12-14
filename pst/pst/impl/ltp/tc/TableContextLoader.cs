using pst.interfaces.ltp.tc;
using pst.encodables.ltp.tc;
using pst.interfaces.ltp.bth;
using pst.interfaces.ltp.hn;
using pst.interfaces;
using pst.encodables.ltp.bth;
using System.Linq;
using pst.interfaces.io;
using pst.encodables.ndb.btree;
using pst.encodables.ndb;
using pst.encodables.ltp.hn;
using pst.utilities;
using System;
using System.Collections.Generic;

namespace pst.impl.ltp.tc
{
    class TableContextLoader : ITableContextLoader
    {
        private readonly IConverter<DataRecord, TCROWID> dataRecordToTCROWIDConverter;
        private readonly IBTreeOnHeapLeafKeysEnumerator bthLeafKeysEnumerator;
        private readonly IHeapOnNodeLoader heapOnNodeLoader;
        private readonly IDecoder<TCINFO> tcinfoDecoder;
        private readonly IDecoder<HNID> hnidDecoder;

        public TableContextLoader(
            IConverter<DataRecord, TCROWID> dataRecordToTCROWIDConverter,
            IBTreeOnHeapLeafKeysEnumerator bthLeafKeysEnumerator,
            IHeapOnNodeLoader heapOnNodeLoader,
            IDecoder<TCINFO> tcinfoDecoder,
            IDecoder<HNID> hnidDecoder)
        {
            this.dataRecordToTCROWIDConverter = dataRecordToTCROWIDConverter;
            this.bthLeafKeysEnumerator = bthLeafKeysEnumerator;
            this.heapOnNodeLoader = heapOnNodeLoader;
            this.tcinfoDecoder = tcinfoDecoder;
            this.hnidDecoder = hnidDecoder;
        }

        public TableRow[] Load(
            IDataBlockReader<LBBTEntry> reader,
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry)
        {
            var heapOnNode =
                heapOnNodeLoader.Load(
                    reader,
                    blockIdToEntryMapping,
                    blockEntry);

            var tcinfo =
                tcinfoDecoder.Decode(
                    heapOnNode.Root);

            var dataRecords =
                bthLeafKeysEnumerator.Enumerate(
                    heapOnNode.ChangeRoot(tcinfo.RowIndex));

            var tcrowIds =
                dataRecords
                .Select(dataRecordToTCROWIDConverter.Convert)
                .ToArray();

            var rowMatrixHnid =
                hnidDecoder.Decode(tcinfo.RowMatrix);

            if (rowMatrixHnid.IsHID)
            {
                var heapItem =
                    heapOnNode.GetItem(rowMatrixHnid.HID);

                var rows =
                    ParseRows(heapItem, tcinfo.GroupsOffsets[3]);

                return
                    rows
                    .Select(r => Convert(r, tcinfo.ColumnDescriptors))
                    .ToArray();
            }

            return null;
        }

        private TableRow Convert(BinaryData encodedRow, TCOLDESC[] columnDescriptions)
        {
            var parser = BinaryDataParser.OfValue(encodedRow);

            var values = new List<BinaryData>();

            foreach (var c in columnDescriptions)
            {
                values.Add(parser.TakeAt(c.DataOffset, c.DataSize));
            }

            return new TableRow(values.ToArray());
        }

        private BinaryData[] ParseRows(BinaryData data, int rowLength)
        {
            var numberOfRows =
                (int)
                Math.Floor((double)data.Length / rowLength);

            var parser = BinaryDataParser.OfValue(data);

            var rows = new List<BinaryData>();

            for (var i = 0; i < numberOfRows; i++)
            {
                rows.Add(parser.TakeAndSkip(rowLength));
            }

            return rows.ToArray();
        }
    }
}

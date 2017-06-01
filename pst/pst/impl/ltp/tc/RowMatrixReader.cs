using pst.core;
using pst.encodables.ltp.hn;
using pst.encodables.ltp.tc;
using pst.encodables.ndb;
using pst.encodables.ndb.blocks.subnode;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.io;
using pst.interfaces.ltp.hn;
using pst.interfaces.ltp.tc;
using pst.interfaces.ndb;
using System;

namespace pst.impl.ltp.tc
{
    class RowMatrixReader<TRowId>
        : IRowMatrixReader<TRowId>
    {
        private readonly IDataTreeLeafNodesEnumerator dataTreeLeafNodesEnumerator;
        private readonly IRowValuesExtractor rowValuesExtractor;
        private readonly IHeapOnNodeReader heapOnNodeReader;
        private readonly IDecoder<TCINFO> tcinfoDecoder;
        private readonly IRowIndexReader<TRowId> rowIndexReader;
        private readonly IDecoder<HNID> hnidDecoder;
        private readonly IDataBlockReader<LBBTEntry> dataBlockReader;

        public RowMatrixReader(
            IDataTreeLeafNodesEnumerator dataTreeLeafNodesEnumerator,
            IRowValuesExtractor rowValuesExtractor,
            IHeapOnNodeReader heapOnNodeReader,
            IDecoder<TCINFO> tcinfoDecoder,
            IRowIndexReader<TRowId> rowIndexReader,
            IDecoder<HNID> hnidDecoder,
            IDataBlockReader<LBBTEntry> dataBlockReader)
        {
            this.dataTreeLeafNodesEnumerator = dataTreeLeafNodesEnumerator;
            this.rowValuesExtractor = rowValuesExtractor;
            this.heapOnNodeReader = heapOnNodeReader;
            this.rowIndexReader = rowIndexReader;
            this.tcinfoDecoder = tcinfoDecoder;
            this.hnidDecoder = hnidDecoder;
            this.dataBlockReader = dataBlockReader;
        }

        public Maybe<TableRow> GetRow(
            IMapper<NID, SLEntry> nidToSLEntryMapping,
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry,
            TRowId rowId)
        {
            var hnHeader =
                heapOnNodeReader
                .GetHeapOnNodeHeader(blockIdToEntryMapping, blockEntry);

            var tcInfo =
                tcinfoDecoder
                .Decode(
                    heapOnNodeReader
                    .GetHeapItem(blockIdToEntryMapping, blockEntry, hnHeader.UserRoot));

            var rowMatrixHnid = hnidDecoder.Decode(tcInfo.RowMatrix);

            if (rowMatrixHnid.IsZero)
            {
                return Maybe<TableRow>.NoValue();
            }

            var tcRowId =
                rowIndexReader
                .GetRowId(blockIdToEntryMapping, blockEntry, rowId);

            if (tcRowId.HasNoValue)
            {
                return Maybe<TableRow>.NoValue();
            }

            if (rowMatrixHnid.IsHID)
            {
                var heapItem =
                    heapOnNodeReader
                    .GetHeapItem(blockIdToEntryMapping, blockEntry, rowMatrixHnid.HID);

                var encodedRows =
                    heapItem.Slice(tcInfo.GroupsOffsets[3]);

                var tableRow =
                    new TableRow(
                        tcRowId.Value.RowId,
                        rowValuesExtractor.Extract(
                            encodedRows[tcRowId.Value.RowIndex],
                            tcInfo.ColumnDescriptors));

                return Maybe<TableRow>.OfValue(tableRow);
            }
            else
            {
                var subnodeBlockId =
                    nidToSLEntryMapping
                    .Map(rowMatrixHnid.NID).DataBlockId;

                var lbbtEntryForSubnode =
                    blockIdToEntryMapping.Map(subnodeBlockId);

                var dataBlocks =
                    dataTreeLeafNodesEnumerator
                    .Enumerate(blockIdToEntryMapping, lbbtEntryForSubnode);

                var numberOfRowsPerBlock =
                    Math.Floor((double)(8 * 1024 - 16) / tcInfo.GroupsOffsets[3]);

                var blockIndex =
                    (int)(tcRowId.Value.RowIndex / numberOfRowsPerBlock);

                var rowIndex =
                    tcRowId.Value.RowIndex % numberOfRowsPerBlock;

                var dataBlockId = dataBlocks[blockIndex];

                var lbbEntryForDataBlock =
                    blockIdToEntryMapping.Map(dataBlockId);

                var dataBlock =
                    dataBlockReader.Read(lbbEntryForDataBlock, lbbEntryForDataBlock.GetBlockSize());

                var tableRow =
                    new TableRow(
                        tcRowId.Value.RowId,
                        rowValuesExtractor.Extract(dataBlock, tcInfo.ColumnDescriptors));

                return Maybe<TableRow>.OfValue(tableRow);
            }
        }
    }
}

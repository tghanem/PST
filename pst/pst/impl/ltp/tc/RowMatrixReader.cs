using pst.core;
using pst.encodables.ltp.hn;
using pst.encodables.ltp.tc;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.io;
using pst.interfaces.ltp.hn;
using pst.interfaces.ltp.tc;
using pst.interfaces.ndb;
using System;
using System.Linq;

namespace pst.impl.ltp.tc
{
    class RowMatrixReader<TRowId> : IRowMatrixReader<TRowId>
    {
        private readonly IHeapOnNodeReader heapOnNodeReader;
        private readonly IRowValuesExtractor rowValuesExtractor;
        private readonly ISubNodesEnumerator subnodesEnumerator;
        private readonly IRowIndexReader<TRowId> rowIndexReader;
        private readonly IDataTreeLeafBIDsEnumerator dataTreeLeafNodesEnumerator;

        private readonly IDecoder<HNID> hnidDecoder;
        private readonly IDecoder<TCINFO> tcinfoDecoder;

        private readonly IDataBlockReader dataBlockReader;

        public RowMatrixReader(
            IHeapOnNodeReader heapOnNodeReader,
            IRowValuesExtractor rowValuesExtractor,
            ISubNodesEnumerator subnodesEnumerator,
            IRowIndexReader<TRowId> rowIndexReader,
            IDataTreeLeafBIDsEnumerator dataTreeLeafNodesEnumerator,
            IDecoder<HNID> hnidDecoder,
            IDecoder<TCINFO> tcinfoDecoder,
            IDataBlockReader dataBlockReader)
        {
            this.dataTreeLeafNodesEnumerator = dataTreeLeafNodesEnumerator;
            this.rowValuesExtractor = rowValuesExtractor;
            this.heapOnNodeReader = heapOnNodeReader;
            this.rowIndexReader = rowIndexReader;
            this.tcinfoDecoder = tcinfoDecoder;
            this.hnidDecoder = hnidDecoder;
            this.subnodesEnumerator = subnodesEnumerator;
            this.dataBlockReader = dataBlockReader;
        }

        public Maybe<TableRow> GetRow(BID nodeDataBlockId, BID subnodeDataBlockId, TRowId rowId)
        {
            var hnHeader =
                heapOnNodeReader.GetHeapOnNodeHeader(nodeDataBlockId);

            var userRootHeapItem =
                heapOnNodeReader.GetHeapItem(nodeDataBlockId, hnHeader.UserRoot);

            var tcInfo =
                tcinfoDecoder.Decode(userRootHeapItem);

            var rowMatrixHnid =
                hnidDecoder.Decode(tcInfo.RowMatrix);

            if (rowMatrixHnid.IsZero)
            {
                return Maybe<TableRow>.NoValue();
            }

            var tcRowId =
                rowIndexReader.GetRowId(nodeDataBlockId, rowId);

            if (tcRowId.HasNoValue)
            {
                return Maybe<TableRow>.NoValue();
            }

            if (rowMatrixHnid.IsHID)
            {
                var heapItem =
                    heapOnNodeReader.GetHeapItem(nodeDataBlockId, rowMatrixHnid.HID);

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
                var subnodeIds =
                    subnodesEnumerator.Enumerate(subnodeDataBlockId);

                var slEntry =
                    subnodeIds.First(e => e.LocalSubnodeId.Value == rowMatrixHnid.NID.Value);

                var dataBlocks =
                    dataTreeLeafNodesEnumerator.Enumerate(slEntry.DataBlockId);

                var numberOfRowsPerBlock =
                    Math.Floor((double)(8 * 1024 - 16) / tcInfo.GroupsOffsets[3]);

                var blockIndex =
                    (int)(tcRowId.Value.RowIndex / numberOfRowsPerBlock);

                var dataBlock =
                    dataBlockReader.Read(dataBlocks[blockIndex]);

                var tableRow =
                    new TableRow(
                        tcRowId.Value.RowId,
                        rowValuesExtractor.Extract(dataBlock, tcInfo.ColumnDescriptors));

                return Maybe<TableRow>.OfValue(tableRow);
            }
        }
    }
}

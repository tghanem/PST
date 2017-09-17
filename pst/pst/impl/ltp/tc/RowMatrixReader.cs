using pst.core;
using pst.encodables.ltp.hn;
using pst.encodables.ltp.tc;
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
        private readonly INodeEntryFinder nodeEntryFinder;
        private readonly IRowIndexReader<TRowId> rowIndexReader;
        private readonly IDataBlockEntryFinder dataBlockEntryFinder;
        private readonly IDecoder<HNID> hnidDecoder;
        private readonly IDecoder<TCINFO> tcinfoDecoder;
        private readonly IDataBlockReader dataBlockReader;

        public RowMatrixReader(
            IHeapOnNodeReader heapOnNodeReader,
            IRowValuesExtractor rowValuesExtractor,
            INodeEntryFinder nodeEntryFinder,
            IRowIndexReader<TRowId> rowIndexReader,
            IDataBlockEntryFinder dataBlockEntryFinder,
            IDecoder<HNID> hnidDecoder,
            IDecoder<TCINFO> tcinfoDecoder,
            IDataBlockReader dataBlockReader)
        {
            this.dataBlockEntryFinder = dataBlockEntryFinder;
            this.rowValuesExtractor = rowValuesExtractor;
            this.heapOnNodeReader = heapOnNodeReader;
            this.rowIndexReader = rowIndexReader;
            this.tcinfoDecoder = tcinfoDecoder;
            this.hnidDecoder = hnidDecoder;
            this.nodeEntryFinder = nodeEntryFinder;
            this.dataBlockReader = dataBlockReader;
        }

        public Maybe<TableRow> GetRow(NodePath nodePath, TRowId rowId)
        {
            var nodeEntry =
                nodeEntryFinder.GetEntry(nodePath);

            var hnHeader =
                heapOnNodeReader.GetHeapOnNodeHeader(nodePath);

            var userRootHeapItem =
                heapOnNodeReader.GetHeapItem(nodePath, hnHeader.UserRoot);

            var tcInfo =
                tcinfoDecoder.Decode(userRootHeapItem);

            var rowMatrixHnid =
                hnidDecoder.Decode(tcInfo.RowMatrix);

            if (rowMatrixHnid.IsZero)
            {
                return Maybe<TableRow>.NoValue();
            }

            var tcRowId =
                rowIndexReader.GetRowId(nodePath, rowId);

            if (tcRowId.HasNoValue)
            {
                return Maybe<TableRow>.NoValue();
            }

            if (rowMatrixHnid.IsHID)
            {
                var heapItem =
                    heapOnNodeReader.GetHeapItem(nodePath, rowMatrixHnid.HID);

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
                var slEntry =
                    nodeEntry.Value.ChildNodes.First(e => e.LocalSubnodeId.Value == rowMatrixHnid.NID.Value);

                var numberOfRowsPerBlock =
                    Math.Floor((double)(8 * 1024 - 16) / tcInfo.GroupsOffsets[3]);

                var blockIndex =
                    (int)(tcRowId.Value.RowIndex / numberOfRowsPerBlock);

                var dataBlockTree =
                    dataBlockEntryFinder.Find(slEntry.DataBlockId);

                var actualBlockId = slEntry.DataBlockId;

                if (dataBlockTree.Value.ChildBlockIds.HasValueAnd(childBlockIds => childBlockIds.Length > 0))
                {
                    actualBlockId = dataBlockTree.Value.ChildBlockIds.Value[blockIndex];
                }

                var dataBlock =
                    dataBlockReader.Read(actualBlockId);

                var tableRow =
                    new TableRow(
                        tcRowId.Value.RowId,
                        rowValuesExtractor.Extract(dataBlock, tcInfo.ColumnDescriptors));

                return Maybe<TableRow>.OfValue(tableRow);
            }
        }
    }
}

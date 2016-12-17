using pst.interfaces.ltp.tc;
using pst.encodables.ltp.tc;
using pst.interfaces.ltp.hn;
using pst.interfaces;
using System.Linq;
using pst.interfaces.io;
using pst.encodables.ndb.btree;
using pst.encodables.ndb;
using pst.encodables.ltp.hn;
using System;
using System.Collections.Generic;
using pst.interfaces.ndb;
using pst.encodables.ndb.blocks.subnode;

namespace pst.impl.ltp.tc
{
    class RowMatrixLoader : IRowMatrixLoader
    {
        private readonly IDataTreeLeafNodesEnumerator dataTreeLeafNodesEnumerator;
        private readonly IRowValuesExtractor rowValuesExtractor;
        private readonly IHeapOnNodeLoader heapOnNodeLoader;
        private readonly IDecoder<TCINFO> tcinfoDecoder;
        private readonly IDecoder<HNID> hnidDecoder;

        public RowMatrixLoader(
            IDataTreeLeafNodesEnumerator dataTreeLeafNodesEnumerator,
            IRowValuesExtractor rowValuesExtractor,
            IHeapOnNodeLoader heapOnNodeLoader,
            IDecoder<TCINFO> tcinfoDecoder,
            IDecoder<HNID> hnidDecoder)
        {
            this.dataTreeLeafNodesEnumerator = dataTreeLeafNodesEnumerator;
            this.rowValuesExtractor = rowValuesExtractor;
            this.heapOnNodeLoader = heapOnNodeLoader;
            this.tcinfoDecoder = tcinfoDecoder;
            this.hnidDecoder = hnidDecoder;
        }

        public TableRow[] Load(
            IDataBlockReader<LBBTEntry> reader,
            IMapper<NID, SLEntry> nidToSLEntryMapping,
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

            var rowMatrixHnid =
                hnidDecoder.Decode(tcinfo.RowMatrix);

            if (rowMatrixHnid.IsZero)
            {
                return new TableRow[0];
            }

            var rows = new List<TableRow>();

            if (rowMatrixHnid.IsHID)
            {
                var heapItem =
                    heapOnNode.GetItem(rowMatrixHnid.HID);

                rows
                .AddRange(
                    heapItem.Slice(tcinfo.GroupsOffsets[3])
                    .Select(r => new TableRow(rowValuesExtractor.Extract(r, tcinfo.ColumnDescriptors))));
            }
            else
            {
                var subnodeBlockId =
                    nidToSLEntryMapping.Map(rowMatrixHnid.NID).DataBlockId;

                var lbbtEntryForSubnode =
                    blockIdToEntryMapping.Map(subnodeBlockId);

                var dataBlocks =
                    dataTreeLeafNodesEnumerator
                        .Enumerate(
                            reader,
                            blockIdToEntryMapping,
                            lbbtEntryForSubnode);

                Array.ForEach(
                    dataBlocks,
                    d
                    =>
                    {
                        rows
                        .AddRange(
                            d.Data.Slice(tcinfo.GroupsOffsets[3])
                            .Select(r => new TableRow(rowValuesExtractor.Extract(r, tcinfo.ColumnDescriptors))));
                    });
            }

            return rows.ToArray();
        }
    }
}

using pst.core;
using pst.encodables.ltp.tc;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.ltp.tc;

namespace pst.impl.ltp.tc
{
    class TCReader<TRowId> : ITCReader<TRowId>
    {
        private readonly IRowIndexReader<TRowId> rowIndexReader;
        private readonly IRowMatrixReader<TRowId> rowMatrixReader;

        private readonly IMapper<NID, LNBTEntry> nidToLNBTEntryMapping;
        private readonly IMapper<BID, LBBTEntry> bidToLBBTEntryMapping;

        public TCReader(
            IRowIndexReader<TRowId> rowIndexReader,
            IRowMatrixReader<TRowId> rowMatrixReader,
            IMapper<NID, LNBTEntry> nidToLNBTEntryMapping,
            IMapper<BID, LBBTEntry> bidToLBBTEntryMapping)
        {
            this.rowIndexReader = rowIndexReader;
            this.rowMatrixReader = rowMatrixReader;
            this.nidToLNBTEntryMapping = nidToLNBTEntryMapping;
            this.bidToLBBTEntryMapping = bidToLBBTEntryMapping;
        }

        public TCROWID[] GetAllRowIds(NID nodeId)
        {
            var lnbtEntry = nidToLNBTEntryMapping.Map(nodeId);

            var lbbtEntry = bidToLBBTEntryMapping.Map(lnbtEntry.DataBlockId);

            return rowIndexReader.GetAllRowIds(lbbtEntry);
        }

        public Maybe<TableRow> GetRow(NID nodeId, TRowId rowId)
        {
            var lnbtEntry = nidToLNBTEntryMapping.Map(nodeId);

            var lbbtEntry = bidToLBBTEntryMapping.Map(lnbtEntry.DataBlockId);

            return rowMatrixReader.GetRow(null, lbbtEntry, rowId);
        }
    }
}

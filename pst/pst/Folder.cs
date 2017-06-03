using pst.core;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.ltp.pc;
using pst.interfaces.ltp.tc;
using pst.utilities;
using System.Linq;

namespace pst
{
    public class Folder
    {
        private readonly NID nodeId;
        private readonly IRowIndexReader<NID> rowIndexReader;
        private readonly IRowMatrixReader<NID> rowMatrixReader;
        private readonly IPCBasedPropertyReader pcBasedPropertyReader;
        private readonly IMapper<NID, LNBTEntry> nodeIdToLNBTEntryMapping;
        private readonly IMapper<BID, LBBTEntry> blockIdToLBBTEntryMapping;

        internal Folder(
            NID nodeId,
            IRowIndexReader<NID> rowIndexReader,
            IRowMatrixReader<NID> rowMatrixReader,
            IPCBasedPropertyReader pcBasedPropertyReader,
            IMapper<NID, LNBTEntry> nodeIdToLNBTEntryMapping,
            IMapper<BID, LBBTEntry> blockIdToLBBTEntryMapping)
        {
            this.nodeId = nodeId;
            this.rowIndexReader = rowIndexReader;
            this.rowMatrixReader = rowMatrixReader;
            this.pcBasedPropertyReader = pcBasedPropertyReader;
            this.nodeIdToLNBTEntryMapping = nodeIdToLNBTEntryMapping;
            this.blockIdToLBBTEntryMapping = blockIdToLBBTEntryMapping;
        }

        public Folder[] GetSubFolders()
        {
            var lnbtEntryForHierarchyTable =
                nodeIdToLNBTEntryMapping.Map(nodeId.ChangeType(Globals.NID_TYPE_HIERARCHY_TABLE));

            var bbtEntry =
                blockIdToLBBTEntryMapping.Map(lnbtEntryForHierarchyTable.DataBlockId);

            var rowIds =
                rowIndexReader.GetAllRowIds(bbtEntry);

            return
                rowIds
                .Select(
                    r =>
                    new Folder(
                        r.RowId,
                        rowIndexReader,
                        rowMatrixReader,
                        pcBasedPropertyReader,
                        nodeIdToLNBTEntryMapping,
                        blockIdToLBBTEntryMapping))
                .ToArray();
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            return pcBasedPropertyReader.ReadProperty(nodeId, propertyTag);
        }
    }
}

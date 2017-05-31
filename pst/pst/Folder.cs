using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.io;
using pst.interfaces.ltp.pc;
using pst.interfaces.ltp.tc;
using pst.utilities;
using System.Linq;
using System.Text;

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
        private readonly IDataBlockReader<LBBTEntry> dataBlockReader;

        internal Folder(
            NID nodeId,
            IRowIndexReader<NID> rowIndexReader,
            IRowMatrixReader<NID> rowMatrixReader,
            IPCBasedPropertyReader pcBasedPropertyReader,
            IMapper<NID, LNBTEntry> nodeIdToLNBTEntryMapping,
            IMapper<BID, LBBTEntry> blockIdToLBBTEntryMapping,
            IDataBlockReader<LBBTEntry> dataBlockReader)
        {
            this.nodeId = nodeId;
            this.rowIndexReader = rowIndexReader;
            this.rowMatrixReader = rowMatrixReader;
            this.dataBlockReader = dataBlockReader;
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
                rowIndexReader.GetAllRowIds(dataBlockReader, blockIdToLBBTEntryMapping, bbtEntry);

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
                        blockIdToLBBTEntryMapping,
                        dataBlockReader))
                .ToArray();
        }

        public string DisplayName
        {
            get
            {
                var propertyTag =
                    new PropertyTag(
                        new PropertyId(0x3001),
                        new PropertyType(0x001F));

                var propertyValue =
                    pcBasedPropertyReader
                    .ReadProperty(
                        dataBlockReader,
                        blockIdToLBBTEntryMapping,
                        blockIdToLBBTEntryMapping.Map(nodeIdToLNBTEntryMapping.Map(nodeId).DataBlockId),
                        propertyTag);

                return Encoding.Unicode.GetString(propertyValue.Value.Value.Value);
            }
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}

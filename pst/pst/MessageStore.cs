using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.io;
using pst.interfaces.ltp.pc;
using System.Text;

namespace pst
{
    public class MessageStore
    {
        private readonly NID nodeId;
        private readonly IPCBasedPropertyReader pcBasedPropertyReader;
        private readonly IMapper<NID, LNBTEntry> nodeIdToLNBTEntryMapping;
        private readonly IMapper<BID, LBBTEntry> blockIdToLBBTEntryMapping;
        private readonly IDataBlockReader<LBBTEntry> dataBlockReader;

        internal MessageStore(
            NID nodeId,
            IPCBasedPropertyReader pcBasedPropertyReader,
            IMapper<NID, LNBTEntry> nodeIdToLNBTEntryMapping,
            IMapper<BID, LBBTEntry> blockIdToLBBTEntryMapping,
            IDataBlockReader<LBBTEntry> dataBlockReader)
        {
            this.nodeId = nodeId;
            this.pcBasedPropertyReader = pcBasedPropertyReader;
            this.nodeIdToLNBTEntryMapping = nodeIdToLNBTEntryMapping;
            this.blockIdToLBBTEntryMapping = blockIdToLBBTEntryMapping;
            this.dataBlockReader = dataBlockReader;
        }

        public string DisplayName
        {
            get
            {
                var nbtEntry =
                    nodeIdToLNBTEntryMapping.Map(nodeId);

                var bbtEntry =
                    blockIdToLBBTEntryMapping.Map(nbtEntry.DataBlockId);

                var propertyTag =
                    new PropertyTag(
                        new PropertyId(0x3001),
                        new PropertyType(0x001F));

                var propertyValue =
                    pcBasedPropertyReader.ReadProperty(bbtEntry, propertyTag);

                return Encoding.Unicode.GetString(propertyValue.Value.Value.Value);
            }
        }
    }
}

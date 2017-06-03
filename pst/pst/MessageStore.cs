using pst.core;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.io;
using pst.interfaces.ltp.pc;
using pst.utilities;

namespace pst
{
    public class MessageStore
    {
        private readonly IPCBasedPropertyReader pcBasedPropertyReader;

        private readonly IMapper<NID, LNBTEntry> nodeIdToLNBTEntryMapping;
        private readonly IMapper<BID, LBBTEntry> blockIdToLBBTEntryMapping;
        private readonly IDataBlockReader<LBBTEntry> dataBlockReader;

        internal MessageStore(
            IPCBasedPropertyReader pcBasedPropertyReader,
            IMapper<NID, LNBTEntry> nodeIdToLNBTEntryMapping,
            IMapper<BID, LBBTEntry> blockIdToLBBTEntryMapping,
            IDataBlockReader<LBBTEntry> dataBlockReader)
        {
            this.pcBasedPropertyReader = pcBasedPropertyReader;
            this.nodeIdToLNBTEntryMapping = nodeIdToLNBTEntryMapping;
            this.blockIdToLBBTEntryMapping = blockIdToLBBTEntryMapping;
            this.dataBlockReader = dataBlockReader;
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            var nbtEntry =
                nodeIdToLNBTEntryMapping.Map(Globals.NID_MESSAGE_STORE);

            var bbtEntry =
                blockIdToLBBTEntryMapping.Map(nbtEntry.DataBlockId);

            return pcBasedPropertyReader.ReadProperty(bbtEntry, propertyTag);
        }
    }
}

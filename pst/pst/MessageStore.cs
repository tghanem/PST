using pst.core;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.ltp.pc;
using pst.utilities;

namespace pst
{
    public class MessageStore
    {
        private readonly IMapper<NID, LNBTEntry> nidToLNBTEntryMapper;

        private readonly IPCBasedPropertyReader pcBasedPropertyReader;

        internal MessageStore(
            IMapper<NID, LNBTEntry> nidToLNBTEntryMapper,
            IPCBasedPropertyReader pcBasedPropertyReader)
        {
            this.nidToLNBTEntryMapper = nidToLNBTEntryMapper;
            this.pcBasedPropertyReader = pcBasedPropertyReader;
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            var lnbtEntry =
                nidToLNBTEntryMapper.Map(Globals.NID_MESSAGE_STORE);

            return pcBasedPropertyReader.ReadProperty(lnbtEntry.DataBlockId, lnbtEntry.SubnodeBlockId, propertyTag);
        }
    }
}

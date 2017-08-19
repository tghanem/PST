using pst.core;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.ltp;
using pst.utilities;

namespace pst
{
    public class MessageStore
    {
        private readonly IMapper<NID, LNBTEntry> nidToLNBTEntryMapper;

        private readonly IPropertyReader propertyReader;

        internal MessageStore(
            IMapper<NID, LNBTEntry> nidToLNBTEntryMapper,
            IPropertyReader propertyReader)
        {
            this.nidToLNBTEntryMapper = nidToLNBTEntryMapper;
            this.propertyReader = propertyReader;
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            var lnbtEntry =
                nidToLNBTEntryMapper.Map(Globals.NID_MESSAGE_STORE);

            return propertyReader.ReadProperty(lnbtEntry.DataBlockId, lnbtEntry.SubnodeBlockId, propertyTag);
        }
    }
}

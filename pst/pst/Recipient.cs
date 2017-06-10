using pst.core;
using pst.encodables;
using pst.encodables.ndb;
using pst.interfaces.ltp.tc;

namespace pst
{
    public class Recipient
    {
        private readonly BID recipientTableBlockId;
        private readonly BID recipientTableSubnodeBlockId;
        private readonly Tag recipientRowId;

        private readonly ITCReader<Tag> tcReader;

        internal Recipient(
            BID recipientTableBlockId,
            BID recipientTableSubnodeBlockId,
            Tag recipientRowId,
            ITCReader<Tag> tcReader)
        {
            this.recipientTableBlockId = recipientTableBlockId;
            this.recipientTableSubnodeBlockId = recipientTableSubnodeBlockId;
            this.recipientRowId = recipientRowId;
            this.tcReader = tcReader;
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            return tcReader.ReadProperty(recipientTableBlockId, recipientTableSubnodeBlockId, recipientRowId, propertyTag);
        }
    }
}

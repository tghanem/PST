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

        private readonly ITCReader<Tag> tableContextReader;

        internal Recipient(
            BID recipientTableBlockId,
            BID recipientTableSubnodeBlockId,
            Tag recipientRowId,
            ITCReader<Tag> tableContextReader)
        {
            this.recipientTableBlockId = recipientTableBlockId;
            this.recipientTableSubnodeBlockId = recipientTableSubnodeBlockId;
            this.recipientRowId = recipientRowId;
            this.tableContextReader = tableContextReader;
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            return
                tableContextReader.ReadProperty(
                    recipientTableBlockId,
                    recipientTableSubnodeBlockId,
                    recipientRowId,
                    propertyTag);
        }
    }
}

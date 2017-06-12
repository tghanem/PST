using pst.core;
using pst.encodables.ndb;
using pst.interfaces.ltp.pc;
using pst.interfaces.ndb;
using pst.utilities;
using System.Linq;

namespace pst
{
    public class Attachment
    {
        private readonly BID attachmentDataBlockId;
        private readonly BID attachmentSubnodeBlockId;
        private readonly IPCBasedPropertyReader pcBasedPropertyReader;

        internal Attachment(
            BID attachmentDataBlockId,
            BID attachmentSubnodeBlockId,
            IPCBasedPropertyReader pcBasedPropertyReader)
        {
            this.attachmentDataBlockId = attachmentDataBlockId;
            this.attachmentSubnodeBlockId = attachmentSubnodeBlockId;
            this.pcBasedPropertyReader = pcBasedPropertyReader;
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            return
                pcBasedPropertyReader.ReadProperty(
                    attachmentDataBlockId,
                    attachmentSubnodeBlockId,
                    propertyTag);
        }
    }
}

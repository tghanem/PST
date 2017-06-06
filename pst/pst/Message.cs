using pst.core;
using pst.encodables.ndb;
using pst.interfaces.ltp.pc;

namespace pst
{
    public class Message
    {
        private readonly NID nodeId;
        private readonly IPCBasedPropertyReader pcBasedPropertyReader;

        internal Message(NID nodeId, IPCBasedPropertyReader pcBasedPropertyReader)
        {
            this.nodeId = nodeId;
            this.pcBasedPropertyReader = pcBasedPropertyReader;
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            return pcBasedPropertyReader.ReadProperty(nodeId, propertyTag);
        }
    }
}

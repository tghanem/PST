using pst.core;
using pst.interfaces.ltp.pc;
using pst.utilities;

namespace pst
{
    public class MessageStore
    {
        private readonly IPCBasedPropertyReader pcBasedPropertyReader;

        internal MessageStore(IPCBasedPropertyReader pcBasedPropertyReader)
        {
            this.pcBasedPropertyReader = pcBasedPropertyReader;
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            return pcBasedPropertyReader.ReadProperty(Globals.NID_MESSAGE_STORE, propertyTag);
        }
    }
}

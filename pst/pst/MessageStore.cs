using pst.interfaces.ltp;
using pst.interfaces.messaging;
using pst.interfaces.messaging.changetracking;
using pst.interfaces.messaging.changetracking.model;
using pst.utilities;

namespace pst
{
    public class MessageStore : ObjectBase
    {
        internal static readonly ObjectPath StorePath = new ObjectPath(new[] { Constants.NID_MESSAGE_STORE });

        internal MessageStore(
            IObjectTracker objectTracker,
            IPropertyNameToIdMap propertyNameToIdMap,
            IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader) : base(StorePath, objectTracker, propertyNameToIdMap, propertyContextBasedPropertyReader)
        {
        }
    }
}

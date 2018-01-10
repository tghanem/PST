using pst.interfaces.ltp;
using pst.interfaces.messaging;
using pst.interfaces.messaging.changetracking;
using pst.interfaces.model;
using pst.utilities;

namespace pst
{
    public class MessageStore : ObjectBase
    {
        internal static readonly NodePath StorePath = NodePath.OfValue(AllocatedNodeId.OfValue(Constants.NID_MESSAGE_STORE));

        internal MessageStore(
            IChangesTracker changesTracker,
            IPropertyNameToIdMap propertyNameToIdMap,
            IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader) : base(StorePath, changesTracker, propertyNameToIdMap, propertyContextBasedPropertyReader)
        {
        }
    }
}

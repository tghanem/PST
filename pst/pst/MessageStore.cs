using pst.interfaces.ltp;
using pst.interfaces.ltp.pc;
using pst.interfaces.messaging.model;
using pst.interfaces.messaging.model.changetracking;
using pst.utilities;

namespace pst
{
    public class MessageStore : ObjectBase
    {
        private static readonly NodePath StorePath = NodePath.OfValue(AllocatedNodeId.OfValue(Constants.NID_MESSAGE_STORE));

        internal MessageStore(
            IChangesTracker changesTracker,
            IPropertyNameToIdMap propertyNameToIdMap,
            IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader) : base(StorePath, ObjectTypes.Store, changesTracker, propertyNameToIdMap, propertyContextBasedPropertyReader)
        {
        }
    }
}

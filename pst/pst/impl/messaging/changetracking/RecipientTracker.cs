using pst.core;
using pst.encodables.ndb;
using pst.interfaces.messaging.changetracking;
using pst.interfaces.messaging.changetracking.model;
using pst.utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pst.impl.messaging.changetracking
{
    class RecipientTracker : IRecipientTracker
    {
        private readonly IDictionary<ObjectPath, RecipientTableTrackingObject> trackedRecipientTables;

        public RecipientTracker(IDictionary<ObjectPath, RecipientTableTrackingObject> trackedRecipientTables)
        {
            this.trackedRecipientTables = trackedRecipientTables;
        }

        public void TrackRecipientTable(ObjectPath messageObjectPath, NID recipientTableNodeId, ObjectStates tableState)
        {
            trackedRecipientTables.Add(
                messageObjectPath,
                new RecipientTableTrackingObject(recipientTableNodeId, ObjectTypes.RecipientTable, tableState));
        }

        public void TrackRecipient(ObjectPath messageObjectPath, NID recipientTableNodeId, int recipientRowId, ObjectStates recipientState)
        {
            trackedRecipientTables[messageObjectPath].TrackedRecipients.Add(
                recipientRowId,
                new TrackingObject(ObjectTypes.Recipient, recipientState));
        }

        public NID GetTrackedRecipientTable(ObjectPath messageObjectPath)
        {
            return trackedRecipientTables[messageObjectPath].RecipientTableNodeId;
        }

        public int[] GetTrackedRecipients(ObjectPath messageObjectPath, NID recipientTableNodeId, Func<ObjectStates, bool> recipientStatePredicate)
        {
            return
                trackedRecipientTables[messageObjectPath]
                .TrackedRecipients
                .Where(r => recipientStatePredicate(r.Value.State))
                .Select(r => r.Key)
                .ToArray();
        }

        public void SetProperty(ObjectPath messageObjectPath, NID recipientTableNodeId, int recipientRowId, PropertyTag propertyTag, PropertyValue propertyValue)
        {
            trackedRecipientTables[messageObjectPath]
                .TrackedRecipients[recipientRowId]
                .UpdateProperty(propertyTag, o => o.SetProperty(propertyTag, propertyValue));
        }

        public void DeleteProperty(ObjectPath messageObjectPath, NID recipientTableNodeId, int recipientRowId, PropertyTag propertyTag)
        {
            trackedRecipientTables[messageObjectPath]
                .TrackedRecipients[recipientRowId]
                .UpdateProperty(propertyTag, o => o.DeleteProperty(propertyTag));
        }

        public Maybe<PropertyValue> GetProperty(ObjectPath messageObjectPath, NID recipientTableNodeId, int recipientRowId, PropertyTag propertyTag, Func<Maybe<PropertyValue>> untrackedPropertyValueReader)
        {
            return
                trackedRecipientTables[messageObjectPath]
                .TrackedRecipients[recipientRowId]
                .GetProperty(propertyTag, untrackedPropertyValueReader);
        }
    }
}

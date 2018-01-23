using System.Collections.Generic;
using pst.encodables.ndb;

namespace pst.interfaces.messaging.changetracking.model
{
    class RecipientTableTrackingObject : TrackingObject
    {
        public RecipientTableTrackingObject(NID recipientTableNodeId, ObjectTypes type, ObjectStates state) : base(type, state)
        {
            RecipientTableNodeId = recipientTableNodeId;
            TrackedRecipients = new Dictionary<int, TrackingObject>();
        }

        public NID RecipientTableNodeId { get; }

        public IDictionary<int, TrackingObject> TrackedRecipients { get; }
    }
}
using pst.core;

namespace pst.interfaces.messaging.changetracking.model
{
    class AttachmentTrackingObject : NodeTrackingObject
    {
        public AttachmentTrackingObject(
            ObjectPath path,
            ObjectTypes type,
            ObjectStates state,
            Maybe<MessageTrackingObject> embeddedMessageTrackingObject) : base(path, type, state)
        {
            EmbeddedMessageTrackingObject = embeddedMessageTrackingObject;
        }

        public Maybe<MessageTrackingObject> EmbeddedMessageTrackingObject { get; }
    }
}
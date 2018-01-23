namespace pst.interfaces.messaging.changetracking.model
{
    class MessageTrackingObject : NodeTrackingObject
    {
        public MessageTrackingObject(
            ObjectPath path,
            ObjectTypes type,
            ObjectStates state,
            TableContextTrackingObject<TrackingObject> recipientTableTrackingObject,
            TableContextTrackingObject<AttachmentTrackingObject> attachmentTableTrackingObject) : base(path, type, state)
        {
            RecipientTableTrackingObject = recipientTableTrackingObject;
            AttachmentTableTrackingObject = attachmentTableTrackingObject;
        }

        public TableContextTrackingObject<TrackingObject> RecipientTableTrackingObject { get; }

        public TableContextTrackingObject<AttachmentTrackingObject> AttachmentTableTrackingObject { get; }
    }
}
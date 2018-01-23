namespace pst.interfaces.messaging.changetracking.model
{
    class FolderTrackingObject : NodeTrackingObject
    {
        public FolderTrackingObject(
            ObjectPath path,
            ObjectTypes type,
            ObjectStates state,
            TableContextTrackingObject<FolderTrackingObject> hierarchyTableTrackingObject,
            TableContextTrackingObject<MessageTrackingObject> contentsTableTrackingObject,
            TableContextTrackingObject<MessageTrackingObject> faiContentsTableTrackingObject) : base(path, type, state)
        {
            HierarchyTableTrackingObject = hierarchyTableTrackingObject;
            ContentsTableTrackingObject = contentsTableTrackingObject;
            FAIContentsTableTrackingObject = faiContentsTableTrackingObject;
        }

        public TableContextTrackingObject<FolderTrackingObject> HierarchyTableTrackingObject { get; }

        public TableContextTrackingObject<MessageTrackingObject> ContentsTableTrackingObject { get; }

        public TableContextTrackingObject<MessageTrackingObject> FAIContentsTableTrackingObject { get; }
    }
}
using System.Collections.Generic;

namespace pst.interfaces.messaging.changetracking.model
{
    class NodeTrackingObject : TrackingObject
    {
        private readonly List<NodeTrackingObject> children;

        public NodeTrackingObject(ObjectPath path, ObjectTypes type, ObjectStates state) : base(type, state)
        {
            Path = path;
            children = new List<NodeTrackingObject>();
        }

        public ObjectPath Path { get; }

        public NodeTrackingObject[] Children => children.ToArray();

        public void AddChild(NodeTrackingObject child) => children.Add(child);
    }
}
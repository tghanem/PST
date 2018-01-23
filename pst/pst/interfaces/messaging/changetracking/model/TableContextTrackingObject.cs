using System.Collections.Generic;

namespace pst.interfaces.messaging.changetracking.model
{
    class TableContextTrackingObject<TChild> : NodeTrackingObject
    {
        private readonly List<TChild> children;

        public TableContextTrackingObject(
            ObjectPath path,
            ObjectTypes type,
            ObjectStates state) : base(path, type, state)
        {
            children = new List<TChild>();
        }

        public TChild[] Children => children.ToArray();

        public void AddChild(TChild child) => children.Add(child);
    }
}
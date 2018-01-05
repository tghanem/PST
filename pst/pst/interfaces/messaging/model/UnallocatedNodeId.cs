namespace pst.interfaces.messaging.model
{
    class UnallocatedNodeId : NodeId
    {
        public UnallocatedNodeId(int type, int index)
        {
            Type = type;
            Index = index;
        }

        public int Type { get; }

        public int Index { get; }

        public override int Value => Type << 16 | Index;

        public override bool Equals(object obj)
        {
            var nodeId = obj as UnallocatedNodeId;

            if (nodeId == null)
            {
                return false;
            }

            return nodeId.Type == Type && nodeId.Index == Index;
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
}
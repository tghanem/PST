namespace pst.interfaces.messaging.model
{
    class UnallocatedNodeId : NodeId
    {
        private UnallocatedNodeId(int index)
        {
            Index = index;
        }

        public static UnallocatedNodeId OfValue(int index) => new UnallocatedNodeId(index);

        public int Index { get; }

        public override int Value => Index;

        public override bool Equals(object obj)
        {
            var nodeId = obj as UnallocatedNodeId;

            return nodeId?.Index == Index;
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
}
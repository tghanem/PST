using pst.encodables.ndb;

namespace pst.interfaces.messaging.model
{
    class AllocatedNodeId : NodeId
    {
        private AllocatedNodeId(NID nid)
        {
            NID = nid;
        }

        public static AllocatedNodeId OfValue(NID value) => new AllocatedNodeId(value);

        public override int Value => NID.Value;

        public NID NID { get; }

        public AllocatedNodeId ChangeType(int value) => new AllocatedNodeId(NID.ChangeType(value));

        public override bool Equals(object obj)
        {
            var allocatedNodeId = obj as AllocatedNodeId;

            return allocatedNodeId?.NID.Equals(NID) ?? false;
        }

        public override int GetHashCode()
        {
            return NID.GetHashCode();
        }
    }
}
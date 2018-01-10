using pst.encodables.ltp.tc;

namespace pst.interfaces.model
{
    class AssociatedObjectPath
    {
        public NodePath NodePath { get; }

        public TCROWID RowId { get; }

        public AssociatedObjectPath(NodePath nodePath, TCROWID rowId)
        {
            NodePath = nodePath;
            RowId = rowId;
        }

        public bool Equals(AssociatedObjectPath other)
        {
            return (other?.NodePath.Equals(NodePath) ?? false) && other.RowId.Equals(RowId);
        }

        public override bool Equals(object obj)
        {
            var path = obj as AssociatedObjectPath;

            return (path?.NodePath.Equals(NodePath) ?? false) && path.RowId.Equals(RowId);
        }

        public override int GetHashCode()
        {
            var p = 17;
            p = p + 23 * NodePath.GetHashCode();
            p = p + 23 * RowId.GetHashCode();
            return p;
        }
    }
}
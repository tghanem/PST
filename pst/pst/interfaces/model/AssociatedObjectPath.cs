using pst.encodables.ltp.tc;

namespace pst.interfaces.model
{
    class AssociatedObjectPath
    {
        public ObjectPath ObjectPath { get; }

        public TCROWID RowId { get; }

        public AssociatedObjectPath(ObjectPath objectPath, TCROWID rowId)
        {
            ObjectPath = objectPath;
            RowId = rowId;
        }

        public bool Equals(AssociatedObjectPath other)
        {
            return (other?.ObjectPath.Equals(ObjectPath) ?? false) && other.RowId.Equals(RowId);
        }

        public override bool Equals(object obj)
        {
            var path = obj as AssociatedObjectPath;

            return (path?.ObjectPath.Equals(ObjectPath) ?? false) && path.RowId.Equals(RowId);
        }

        public override int GetHashCode()
        {
            var p = 17;
            p = p + 23 * ObjectPath.GetHashCode();
            p = p + 23 * RowId.GetHashCode();
            return p;
        }
    }
}
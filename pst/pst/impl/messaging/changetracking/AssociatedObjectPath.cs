using pst.encodables;
using pst.interfaces.messaging.model;

namespace pst.impl.messaging.changetracking
{
    class AssociatedObjectPath
    {
        public NodePath NodePath { get; }

        public Tag Tag { get; }

        public AssociatedObjectPath(NodePath nodePath, Tag tag)
        {
            NodePath = nodePath;
            Tag = tag;
        }

        public bool Equals(AssociatedObjectPath other)
        {
            return (other?.NodePath.Equals(NodePath) ?? false) && other.Tag.Equals(Tag);
        }

        public override bool Equals(object obj)
        {
            var path = obj as AssociatedObjectPath;

            return (path?.NodePath.Equals(NodePath) ?? false) && path.Tag.Equals(Tag);
        }

        public override int GetHashCode()
        {
            var p = 17;
            p = p + 23 * NodePath.GetHashCode();
            p = p + 23 * Tag.GetHashCode();
            return p;
        }
    }
}
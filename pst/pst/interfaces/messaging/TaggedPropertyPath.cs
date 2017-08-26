using System;
using System.Linq;

namespace pst.interfaces.messaging
{
    class TaggedPropertyPath
    {
        public NodePath NodePath { get; }

        public PropertyTag PropertyTag { get; }

        public TaggedPropertyPath(NodePath nodePath, PropertyTag propertyTag)
        {
            NodePath = nodePath;
            PropertyTag = propertyTag;
        }

        public override bool Equals(object obj)
        {
            var taggedPropertyPath = obj as TaggedPropertyPath;

            if (taggedPropertyPath == null)
            {
                return false;
            }

            return taggedPropertyPath.NodePath.Equals(NodePath) &&
                   taggedPropertyPath.PropertyTag.Equals(PropertyTag);
        }

        public override int GetHashCode()
        {
            return
                BitConverter
                .ToString(
                    NodePath
                    .NodeIds
                    .SelectMany(
                        nid =>
                        BitConverter.GetBytes(nid.Value))
                    .Concat(BitConverter.GetBytes(PropertyTag.Value))
                    .ToArray())
                .ToLower()
                .Replace("-", "")
                .GetHashCode();
        }
    }
}

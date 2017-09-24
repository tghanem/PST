using pst.interfaces.ndb;
using System;
using System.Linq;

namespace pst.interfaces.messaging
{
    class NumericalTaggedPropertyPath
    {
        public NodePath NodePath { get; }

        public NumericalPropertyTag PropertyTag { get; }

        public NumericalTaggedPropertyPath(NodePath nodePath, NumericalPropertyTag propertyTag)
        {
            NodePath = nodePath;
            PropertyTag = propertyTag;
        }

        public override bool Equals(object obj)
        {
            var taggedPropertyPath = obj as NumericalTaggedPropertyPath;

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
                    .Concat(PropertyTag.Set.ToByteArray())
                    .Concat(BitConverter.GetBytes(PropertyTag.Id))
                    .Concat(BitConverter.GetBytes(PropertyTag.Type.Value))
                    .ToArray())
                .ToLower()
                .Replace("-", "")
                .GetHashCode();
        }
    }
}

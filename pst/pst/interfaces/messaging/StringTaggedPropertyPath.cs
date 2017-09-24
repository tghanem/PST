using pst.interfaces.ndb;
using System;
using System.Linq;
using System.Text;

namespace pst.interfaces.messaging
{
    class StringTaggedPropertyPath
    {
        public NodePath NodePath { get; }

        public StringPropertyTag PropertyTag { get; }

        public StringTaggedPropertyPath(NodePath nodePath, StringPropertyTag propertyTag)
        {
            NodePath = nodePath;
            PropertyTag = propertyTag;
        }

        public override bool Equals(object obj)
        {
            var taggedPropertyPath = obj as StringTaggedPropertyPath;

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
                    .Concat(Encoding.UTF8.GetBytes(PropertyTag.Name))
                    .Concat(BitConverter.GetBytes(PropertyTag.Type.Value))
                    .ToArray())
                .ToLower()
                .Replace("-", "")
                .GetHashCode();
        }
    }
}

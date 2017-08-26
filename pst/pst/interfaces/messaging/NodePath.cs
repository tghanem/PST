using pst.encodables.ndb;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pst.interfaces.messaging
{
    class NodePath
    {
        private readonly List<NID> pathNodeIds;

        public NodePath(NID[] pathNodeIds)
        {
            this.pathNodeIds = new List<NID>(pathNodeIds);
        }

        public NodePath Add(NID nodeId)
        {
            var copyNodeIds = new List<NID>(pathNodeIds) { nodeId };

            return new NodePath(copyNodeIds.ToArray());
        }

        public static NodePath OfValue(params NID[] value) => new NodePath(value);

        public NID[] NodeIds => pathNodeIds.ToArray();

        public int Length => pathNodeIds.Count;

        public NID this[int index] => pathNodeIds[index];

        public override bool Equals(object obj)
        {
            var path = obj as NodePath;

            if (path == null || path.pathNodeIds.Count != pathNodeIds.Count)
            {
                return false;
            }

            for (var i = 0; i < path.pathNodeIds.Count; i++)
            {
                if (!path.pathNodeIds[i].Equals(pathNodeIds[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return
                BitConverter
                .ToString(
                    pathNodeIds
                    .SelectMany(
                        nid =>
                        BitConverter.GetBytes(nid.Value))
                    .ToArray())
                .ToLower()
                .Replace("-", "")
                .GetHashCode();
        }
    }
}
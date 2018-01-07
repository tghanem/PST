using pst.encodables.ndb;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pst.interfaces.messaging.model
{
    class NodePath
    {
        private readonly List<NodeId> pathNodeIds;

        private NodePath(NodeId[] pathNodeIds)
        {
            this.pathNodeIds = new List<NodeId>(pathNodeIds);
        }

        public NodePath Add(NodeId nodeId)
        {
            var copyNodeIds = new List<NodeId>(pathNodeIds) { nodeId };

            return new NodePath(copyNodeIds.ToArray());
        }

        public static NodePath OfValue(params NodeId[] value) => new NodePath(value);

        public NodeId[] NodeIds => pathNodeIds.ToArray();

        public NID[] AllocatedIds => pathNodeIds.Cast<AllocatedNodeId>().Select(id => id.NID).ToArray();

        public int Length => pathNodeIds.Count;

        public NodeId this[int index] => pathNodeIds[index];

        public NodeId Id => pathNodeIds[Length - 1];

        public NID AllocatedId => AllocatedIds[Length - 1];

        public bool IsFullyAllocated => pathNodeIds.TrueForAll(id => id is AllocatedNodeId);

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
                        nodeId =>
                        BitConverter.GetBytes(nodeId.Value))
                    .ToArray())
                .ToLower()
                .Replace("-", "")
                .GetHashCode();
        }
    }
}
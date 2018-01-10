using System.Collections.Generic;
using System.Linq;
using pst.encodables.ndb;

namespace pst.interfaces.model
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

        public NID[] AllocatedIds => pathNodeIds.Cast<AllocatedNodeId>().Select(id => id.NID).ToArray();

        public NID AllocatedId => AllocatedIds[pathNodeIds.Count - 1];

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
            var p = 17;
            foreach (var id in pathNodeIds)
            {
                p = p + 23 * id.Value;
            }
            return p;
        }
    }
}
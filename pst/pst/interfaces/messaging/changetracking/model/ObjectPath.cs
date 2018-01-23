using System.Collections.Generic;
using System.Linq;
using pst.encodables.ndb;

namespace pst.interfaces.messaging.changetracking.model
{
    class ObjectPath
    {
        private readonly List<NID> pathNodeIds;

        public ObjectPath(NID[] pathNodeIds)
        {
            this.pathNodeIds = new List<NID>(pathNodeIds);
        }

        public ObjectPath Add(NID nid)
        {
            var copyNodeIds = new List<NID>(pathNodeIds) { nid };

            return new ObjectPath(copyNodeIds.ToArray());
        }

        public bool HasParent => pathNodeIds.Count > 1;

        public ObjectPath RootObjectPath => new ObjectPath(new[] { pathNodeIds[0] });

        public ObjectPath ParentObjectPath => new ObjectPath(pathNodeIds.Take(pathNodeIds.Count - 1).ToArray());

        public NID[] Ids => pathNodeIds.ToArray();

        public NID LocalNodeId => pathNodeIds[pathNodeIds.Count - 1];

        public override bool Equals(object obj)
        {
            var path = obj as ObjectPath;

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
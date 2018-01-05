using pst.encodables.ndb;
using pst.encodables.ndb.blocks.subnode;
using System.Linq;

namespace pst.interfaces.ndb
{
    class NodeEntry
    {
        public NodeEntry(BID nodeDataBlockId, BID subnodeDataBlockId, SLEntry[] childNodes)
        {
            NodeDataBlockId = nodeDataBlockId;
            SubnodeDataBlockId = subnodeDataBlockId;
            ChildNodes = childNodes;
        }

        public BID NodeDataBlockId { get; }

        public BID SubnodeDataBlockId { get; }

        public SLEntry[] ChildNodes { get; }

        public SLEntry GetChildOfType(int type)
        {
            return ChildNodes.First(s => s.LocalSubnodeId.Type == type);
        }
    }
}
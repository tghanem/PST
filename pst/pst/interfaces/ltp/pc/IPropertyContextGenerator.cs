using pst.core;
using pst.encodables.ndb;
using System;

namespace pst.interfaces.ltp.pc
{
    class DataTreeWithPossibleSubnodes
    {
        public DataTreeWithPossibleSubnodes(BID dataTreeRootBlockId, Maybe<Tuple<NID, BID>[]> subnodes)
        {
            DataTreeRootBlockId = dataTreeRootBlockId;
            Subnodes = subnodes;
        }

        public BID DataTreeRootBlockId { get; }

        public Maybe<Tuple<NID, BID>[]> Subnodes { get; }
    }

    interface IPropertyContextGenerator
    {
        DataTreeWithPossibleSubnodes Generate(Tuple<PropertyTag, PropertyValue>[] properties);
    }
}
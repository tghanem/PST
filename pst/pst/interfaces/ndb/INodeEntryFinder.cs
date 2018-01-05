using pst.core;
using pst.encodables.ndb;

namespace pst.interfaces.ndb
{
    interface INodeEntryFinder
    {
        Maybe<NodeEntry> GetEntry(NID[] nodePath);
    }
}
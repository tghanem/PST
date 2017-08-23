using pst.core;
using pst.encodables.ndb;

namespace pst.interfaces.messaging
{
    interface IReadOnlyFolder
    {
        Maybe<NID[]> GetNodeIdsForSubFolders(NID folderNodeId);

        Maybe<NID[]> GetNodeIdsForMessages(NID folderNodeId);
    }
}
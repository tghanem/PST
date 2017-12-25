using pst.core;
using pst.encodables.ndb;

namespace pst.interfaces.messaging
{
    interface IFolder
    {
        NID NewFolderNodeId();

        Maybe<NID[]> GetNodeIdsForSubFolders(NID folderNodeId);

        Maybe<NID[]> GetNodeIdsForContents(NID folderNodeId);
    }
}
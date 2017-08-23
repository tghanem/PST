using pst.core;
using pst.encodables;
using pst.encodables.ndb;

namespace pst.interfaces.messaging
{
    interface IReadOnlyMessage
    {
        Maybe<NID> GetRecipientTableNodeId(NID[] messageNodePath);

        Maybe<Tag[]> GetTagsForRecipients(NID[] messageNodePath);

        Maybe<NID[]> GetNodeIdsForAttachments(NID[] messageNodePath);
    }
}
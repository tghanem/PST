using pst.core;
using pst.encodables;
using pst.encodables.ndb;

namespace pst.interfaces.messaging
{
    interface IReadOnlyMessage
    {
        Maybe<NID> GetRecipientTableNodeId(NodePath messageNodePath);

        Maybe<Tag[]> GetTagsForRecipients(NodePath messageNodePath);

        Maybe<NID[]> GetNodeIdsForAttachments(NodePath messageNodePath);
    }
}
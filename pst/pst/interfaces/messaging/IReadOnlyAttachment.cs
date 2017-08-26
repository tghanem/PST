using pst.core;
using pst.encodables.ndb;

namespace pst.interfaces.messaging
{
    interface IReadOnlyAttachment
    {
        Maybe<NID> GetEmbeddedMessageNodeId(NodePath attachmentNodePath);
    }
}
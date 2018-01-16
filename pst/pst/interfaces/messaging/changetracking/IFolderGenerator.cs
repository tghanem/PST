using pst.encodables.ndb;

namespace pst.interfaces.messaging.changetracking
{
    interface IFolderGenerator
    {
        NID Generate(NodeTrackingObject trackingObject);
    }
}
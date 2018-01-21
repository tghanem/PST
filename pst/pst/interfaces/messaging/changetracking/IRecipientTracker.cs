using pst.core;
using pst.encodables.ndb;
using pst.interfaces.model;
using System;

namespace pst.interfaces.messaging.changetracking
{
    interface IRecipientTracker
    {
        void TrackRecipientTable(
            ObjectPath messageObjectPath,
            NID recipientTableNodeId,
            ObjectStates tableState);

        void TrackRecipient(
            ObjectPath messageObjectPath,
            NID recipientTableNodeId,
            int recipientRowId,
            ObjectStates recipientState);

        NID GetTrackedRecipientTable(
            ObjectPath messageObjectPath);

        int[] GetTrackedRecipients(
            ObjectPath messageObjectPath,
            NID recipientTableNodeId,
            Func<ObjectStates, bool> recipientStatePredicate);

        void SetProperty(
            ObjectPath messageObjectPath,
            NID recipientTableNodeId,
            int recipientRowId,
            PropertyTag propertyTag,
            PropertyValue propertyValue);

        void DeleteProperty(
            ObjectPath messageObjectPath,
            NID recipientTableNodeId,
            int recipientRowId,
            PropertyTag propertyTag);

        Maybe<PropertyValue> GetProperty(
            ObjectPath messageObjectPath,
            NID recipientTableNodeId,
            int recipientRowId,
            PropertyTag propertyTag,
            Func<Maybe<PropertyValue>> untrackedPropertyValueReader);
    }
}
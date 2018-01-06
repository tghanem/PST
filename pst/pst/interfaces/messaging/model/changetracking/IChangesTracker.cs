using pst.core;
using System;

namespace pst.interfaces.messaging.model.changetracking
{
    interface IChangesTracker
    {
        Maybe<PropertyValue> ReadProperty(NodePath nodePath, ObjectTypes objectType, PropertyTag tag, Func<Maybe<PropertyValue>> untrackedPropertyValueReader);
    }
}
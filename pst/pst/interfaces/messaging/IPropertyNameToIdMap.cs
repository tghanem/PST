using pst.core;
using System;

namespace pst.interfaces.ltp
{
    interface IPropertyNameToIdMap
    {
        Maybe<PropertyId> GetPropertyId(Guid propertySet, int numericalId);

        Maybe<PropertyId> GetPropertyId(Guid propertySet, string propertyName);
    }
}
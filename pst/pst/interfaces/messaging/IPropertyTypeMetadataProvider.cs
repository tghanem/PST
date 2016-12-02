﻿namespace pst.interfaces.messaging
{
    interface IPropertyTypeMetadataProvider
    {
        bool IsFixedLength(PropertyType propertyType);

        bool IsVariableLength(PropertyType propertyType);

        int GetFixedLengthTypeSize(PropertyType propertyType);
    }
}

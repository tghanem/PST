namespace pst.interfaces.ltp
{
    interface IPropertyTypeMetadataProvider
    {
        bool IsFixedLength(PropertyType propertyType);

        bool IsVariableLength(PropertyType propertyType);

        bool IsMultiValueFixedLength(PropertyType propertyType);

        bool IsMultiValueVariableLength(PropertyType propertyType);

        int GetFixedLengthTypeSize(PropertyType propertyType);
    }
}

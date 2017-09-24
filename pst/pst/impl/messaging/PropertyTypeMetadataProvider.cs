using pst.interfaces.ltp;
using pst.utilities;
using System;

namespace pst.impl.ltp
{
    class PropertyTypeMetadataProvider : IPropertyTypeMetadataProvider
    {
        public bool IsMultiValueFixedLength(PropertyType propertyType)
        {
            return
                propertyType.Value == Constants.PtypMultipleInteger16 ||
                propertyType.Value == Constants.PtypMultipleInteger32 ||
                propertyType.Value == Constants.PtypMultipleFloating32 ||
                propertyType.Value == Constants.PtypMultipleFloating64 ||
                propertyType.Value == Constants.PtypMultipleCurrency ||
                propertyType.Value == Constants.PtypMultipleFloatingTime ||
                propertyType.Value == Constants.PtypMultipleInteger64 ||
                propertyType.Value == Constants.PtypMultipleTime ||
                propertyType.Value == Constants.PtypMultipleGuid;
        }

        public bool IsMultiValueVariableLength(PropertyType propertyType)
        {
            return
                propertyType.Value == Constants.PtypMultipleString ||
                propertyType.Value == Constants.PtypMultipleBinary ||
                propertyType.Value == Constants.PtypMultipleString8;
        }

        public int GetFixedLengthTypeSize(PropertyType propertyType)
        {
            if (propertyType.Value == Constants.PtypInteger16)
                return 2;
            else if (propertyType.Value == Constants.PtypInteger32)
                return 4;
            else if (propertyType.Value == Constants.PtypFloating32)
                return 4;
            else if (propertyType.Value == Constants.PtypFloating64)
                return 8;
            else if (propertyType.Value == Constants.PtypCurrency)
                return 8;
            else if (propertyType.Value == Constants.PtypFloatingTime)
                return 8;
            else if (propertyType.Value == Constants.PtypErrorCode)
                return 4;
            else if (propertyType.Value == Constants.PtypBoolean)
                return 1;
            else if (propertyType.Value == Constants.PtypInteger64)
                return 8;
            else if (propertyType.Value == Constants.PtypTime)
                return 8;
            else if (propertyType.Value == Constants.PtypGuid)
                return 16;
            else
                throw new Exception($"Property type {propertyType.Value} is not supported");
        }

        public bool IsFixedLength(PropertyType propertyType)
        {
            return
                propertyType.Value == Constants.PtypInteger16 ||
                propertyType.Value == Constants.PtypInteger32 ||
                propertyType.Value == Constants.PtypFloating32 ||
                propertyType.Value == Constants.PtypFloating64 ||
                propertyType.Value == Constants.PtypCurrency ||
                propertyType.Value == Constants.PtypFloatingTime ||
                propertyType.Value == Constants.PtypErrorCode ||
                propertyType.Value == Constants.PtypBoolean ||
                propertyType.Value == Constants.PtypInteger64 ||
                propertyType.Value == Constants.PtypTime ||
                propertyType.Value == Constants.PtypGuid;
        }

        public bool IsVariableLength(PropertyType propertyType)
        {
            return
                propertyType.Value == Constants.PtypString ||
                propertyType.Value == Constants.PtypString8 ||
                propertyType.Value == Constants.PtypBinary;
        }
    }
}

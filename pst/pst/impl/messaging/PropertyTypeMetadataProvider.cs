using pst.interfaces.ltp;
using pst.utilities;
using System;

namespace pst.impl.ltp
{
    class PropertyTypeMetadataProvider : IPropertyTypeMetadataProvider
    {
        public int GetFixedLengthTypeSize(PropertyType propertyType)
        {
            if (propertyType.Value == Globals.PtypInteger16)
                return 2;
            else if (propertyType.Value == Globals.PtypInteger32)
                return 4;
            else if (propertyType.Value == Globals.PtypFloating32)
                return 4;
            else if (propertyType.Value == Globals.PtypFloating64)
                return 8;
            else if (propertyType.Value == Globals.PtypCurrency)
                return 8;
            else if (propertyType.Value == Globals.PtypFloatingTime)
                return 8;
            else if (propertyType.Value == Globals.PtypErrorCode)
                return 4;
            else if (propertyType.Value == Globals.PtypBoolean)
                return 1;
            else if (propertyType.Value == Globals.PtypInteger64)
                return 8;
            else if (propertyType.Value == Globals.PtypTime)
                return 8;
            else if (propertyType.Value == Globals.PtypGuid)
                return 16;
            else
                throw new Exception($"Property type {propertyType.Value} is not supported");
        }

        public bool IsFixedLength(PropertyType propertyType)
        {
            return
                propertyType.Value == Globals.PtypInteger16 ||
                propertyType.Value == Globals.PtypInteger32 ||
                propertyType.Value == Globals.PtypFloating32 ||
                propertyType.Value == Globals.PtypFloating64 ||
                propertyType.Value == Globals.PtypCurrency ||
                propertyType.Value == Globals.PtypFloatingTime ||
                propertyType.Value == Globals.PtypErrorCode ||
                propertyType.Value == Globals.PtypBoolean ||
                propertyType.Value == Globals.PtypInteger64 ||
                propertyType.Value == Globals.PtypTime ||
                propertyType.Value == Globals.PtypGuid;
        }

        public bool IsVariableLength(PropertyType propertyType)
        {
            return
                propertyType.Value == Globals.PtypString ||
                propertyType.Value == Globals.PtypString8 ||
                propertyType.Value == Globals.PtypBinary;
        }
    }
}

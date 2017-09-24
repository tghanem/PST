using pst.utilities;

namespace pst
{
    public class PropertyType
    {
        public static readonly PropertyType PtypInteger16 = new PropertyType(0x0002);
        public static readonly PropertyType PtypInteger32 = new PropertyType(0x0003);
        public static readonly PropertyType PtypFloating32 = new PropertyType(0x0004);
        public static readonly PropertyType PtypFloating64 = new PropertyType(0x0005);
        public static readonly PropertyType PtypCurrency = new PropertyType(0x0006);
        public static readonly PropertyType PtypFloatingTime = new PropertyType(0x0007);
        public static readonly PropertyType PtypErrorCode = new PropertyType(0x000A);
        public static readonly PropertyType PtypBoolean = new PropertyType(0x000B);
        public static readonly PropertyType PtypInteger64 = new PropertyType(0x0014);
        public static readonly PropertyType PtypTime = new PropertyType(0x0040);
        public static readonly PropertyType PtypGuid = new PropertyType(0x0048);

        public static readonly PropertyType PtypMultipleInteger16 = new PropertyType(Constants.PtypMultipleInteger16);
        public static readonly PropertyType PtypMultipleInteger32 = new PropertyType(Constants.PtypMultipleInteger32);
        public static readonly PropertyType PtypMultipleFloating32 = new PropertyType(Constants.PtypMultipleFloating32);
        public static readonly PropertyType PtypMultipleFloating64 = new PropertyType(Constants.PtypMultipleFloating64);
        public static readonly PropertyType PtypMultipleCurrency = new PropertyType(Constants.PtypMultipleCurrency);
        public static readonly PropertyType PtypMultipleFloatingTime = new PropertyType(Constants.PtypMultipleFloatingTime);
        public static readonly PropertyType PtypMultipleInteger64 = new PropertyType(Constants.PtypMultipleInteger64);
        public static readonly PropertyType PtypMultipleTime = new PropertyType(Constants.PtypMultipleTime);
        public static readonly PropertyType PtypMultipleGuid = new PropertyType(Constants.PtypMultipleGuid);

        public static readonly PropertyType PtypString = new PropertyType(0x001f);
        public static readonly PropertyType PtypString8 = new PropertyType(0x001e);
        public static readonly PropertyType PtypBinary = new PropertyType(0x0102);
        public static readonly PropertyType PtypObject = new PropertyType(0x000D);

        public static readonly PropertyType PtypMultipleString = new PropertyType(Constants.PtypMultipleString);
        public static readonly PropertyType PtypMultipleBinary = new PropertyType(Constants.PtypMultipleBinary);
        public static readonly PropertyType PtypMultipleString8 = new PropertyType(Constants.PtypMultipleString8);

        public int Value { get; }

        public PropertyType(int value)
        {
            Value = value;
        }

        public static PropertyType OfValue(int value) => new PropertyType(value);

        public override bool Equals(object obj)
        {
            var type = obj as PropertyType;

            return type?.Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return $"0x{Value:x}".ToLower();
        }
    }
}

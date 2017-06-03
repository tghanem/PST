using pst.encodables.ndb;

namespace pst.utilities
{
    static class Globals
    {
        public static readonly NID NID_MESSAGE_STORE = new NID(0x21);
        public static readonly NID NID_ROOT_FOLDER = new NID(0x122);

        public static readonly int NID_TYPE_HIERARCHY_TABLE = 0x0D;

        public const int NID_TYPE_HID = 0x00;

        public const int PtypInteger16 = 0x0002;
        public const int PtypInteger32 = 0x0003;
        public const int PtypFloating32 = 0x0004;
        public const int PtypFloating64 = 0x0005;
        public const int PtypCurrency = 0x0006;
        public const int PtypFloatingTime = 0x0007;
        public const int PtypErrorCode = 0x000A;
        public const int PtypBoolean = 0x000B;
        public const int PtypInteger64 = 0x0014;
        public const int PtypTime = 0x0040;
        public const int PtypGuid = 0x0048;

        public const int PtypString = 0x001f;
        public const int PtypString8 = 0x001e;

        public const int PtypBinary = 0x0102;
    }
}

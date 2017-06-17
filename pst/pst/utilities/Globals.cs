using pst.encodables.ndb;

namespace pst.utilities
{
    static class Globals
    {
        public static readonly NID NID_MESSAGE_STORE = new NID(0x21);
        public static readonly NID NID_ROOT_FOLDER = new NID(0x122);
        public static readonly NID NID_NAME_TO_ID_MAP = new NID(0x61);

        public const int NID_TYPE_HIERARCHY_TABLE = 0x0D;
        public const int NID_TYPE_CONTENTS_TABLE = 0x0E;
        public const int NID_TYPE_RECIPIENT_TABLE = 0x12;
        public const int NID_TYPE_ATTACHMENT_TABLE = 0x11;
        public const int NID_TYPE_ATTACHMENT = 0x05;

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

        public const int PtypMultipleInteger16 = 0x1002;
        public const int PtypMultipleInteger32 = 0x1003;
        public const int PtypMultipleFloating32 = 0x1004;
        public const int PtypMultipleFloating64 = 0x1005;
        public const int PtypMultipleCurrency = 0x1006;
        public const int PtypMultipleFloatingTime = 0x1007;
        public const int PtypMultipleInteger64 = 0x1014;
        public const int PtypMultipleTime = 0x1040;
        public const int PtypMultipleGuid = 0x1048;

        public const int PtypString = 0x001f;
        public const int PtypString8 = 0x001e;

        public const int PtypBinary = 0x0102;
        public const int PtypObject = 0x000D;

        public const int PtypMultipleBinary = 0x1102;
        public const int PtypMultipleString = 0x101F;
        public const int PtypMultipleString8 = 0x101E;
    }
}

using pst.encodables.ndb;

namespace pst.utilities
{
    static class Constants
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
        public const int NID_TYPE_INTERNAL = 0x01;

        public const int ptypeBBT = 0x80;
        public const int ptypeNBT = 0x81;
        public const int ptypeFMap = 0x82;
        public const int ptypePMap = 0x83;
        public const int ptypeAMap = 0x84;
        public const int ptypeFPMap = 0x85;
        public const int ptypeDL = 0x86;

        public const int bTypeReserved1 = 0x6C;
        public const int bTypeTC = 0x7C;
        public const int bTypeReserved2 = 0x8C;
        public const int bTypeReserved3 = 0x9C;
        public const int bTypeReserved4 = 0xA5;
        public const int bTypeReserved5 = 0xAC;
        public const int bTypeBTH = 0xB5;
        public const int bTypePC = 0xBC;
        public const int bTypeReserved6 = 0xCC;

        //At least 3584 bytes free / data block does not exist
        public const int FILL_LEVEL_EMPTY = 0x0;

        //2560-3584 bytes free
        public const int FILL_LEVEL_1 = 0x1;

        //2048-2560 bytes free
        public const int FILL_LEVEL_2 = 0x2;

        //1792-2048 bytes free
        public const int FILL_LEVEL_3 = 0x3;

        //1536-1792 bytes free
        public const int FILL_LEVEL_4 = 0x4;

        //1280-1536 bytes free
        public const int FILL_LEVEL_5 = 0x5;

        //1024-1280 bytes free
        public const int FILL_LEVEL_6 = 0x6;

        //768-1024 bytes free
        public const int FILL_LEVEL_7 = 0x7;

        //512-768 bytes free
        public const int FILL_LEVEL_8 = 0x8;

        //256-512 bytes free
        public const int FILL_LEVEL_9 = 0x9;

        //128-256 bytes free
        public const int FILL_LEVEL_10 = 0xA;

        //64-128 bytes free
        public const int FILL_LEVEL_11 = 0xB;

        //32-64 bytes free
        public const int FILL_LEVEL_12 = 0xC;

        //16-32 bytes free
        public const int FILL_LEVEL_13 = 0xD;

        //8-16 bytes free
        public const int FILL_LEVEL_14 = 0xE;

        //Data block has less than 8 bytes free
        public const int FILL_LEVEL_FULL = 0xF;

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

        public const int NDB_CRYPT_NONE = 0x00;
        public const int NDB_CRYPT_PERMUTE = 0x01;
        public const int NDB_CRYPT_CYCLIC = 0x02;

        public const int SUDT_NULL = 0x00;
        public const int SUDT_MSG_ADD = 0x01;
        public const int SUDT_MSG_MOD = 0x02;
        public const int SUDT_MSG_DEL = 0x03;
        public const int SUDT_MSG_MOV = 0x04;
        public const int SUDT_FLD_ADD = 0x05;
        public const int SUDT_FLD_MOD = 0x06;
        public const int SUDT_FLD_DEL = 0x07;
        public const int SUDT_FLD_MOV = 0x08;
        public const int SUDT_SRCH_ADD = 0x09;
        public const int SUDT_SRCH_MOD = 0x0A;
        public const int SUDT_SRCH_DEL = 0x0B;
        public const int SUDT_MSG_ROW_MOD = 0x0C;
        public const int SUDT_MSG_SPAM = 0x0D;
        public const int SUDT_IDX_MSG_DEL = 0x0E;
        public const int SUDT_MSG_IDX = 0x0F;

        public static readonly byte[] mpbbCrypt =
        {
            65, 54, 19, 98, 168, 33, 110, 187, 244, 22, 204, 4, 127, 100, 232, 93,
            30, 242, 203, 42, 116, 197, 94, 53, 210, 149, 71, 158, 150, 45, 154,
            136, 76, 125, 132, 63, 219, 172, 49, 182, 72, 95, 246, 196, 216, 57,
            139, 231, 35, 59, 56, 142, 200, 193, 223, 37, 177, 32, 165, 70, 96,
            78, 156, 251, 170, 211, 86, 81, 69, 124, 85, 0, 7, 201, 43, 157, 133,
            155, 9, 160, 143, 173, 179, 15, 99, 171, 137, 75, 215, 167, 21, 90,
            113, 102, 66, 191, 38, 74, 107, 152, 250, 234, 119, 83, 178, 112, 5,
            44, 253, 89, 58, 134, 126, 206, 6, 235, 130, 120, 87, 199, 141, 67,
            175, 180, 28, 212, 91, 205, 226, 233, 39, 79, 195, 8, 114, 128, 207,
            176, 239, 245, 40, 109, 190, 48, 77, 52, 146, 213, 14, 60, 34, 50,
            229, 228, 249, 159, 194, 209, 10, 129, 18, 225, 238, 145, 131, 118,
            227, 151, 230, 97, 138, 23, 121, 164, 183, 220, 144, 122, 92, 140,
            2, 166, 202, 105, 222, 80, 26, 17, 147, 185, 82, 135, 88, 252, 237,
            29, 55, 73, 27, 106, 224, 41, 51, 153, 189, 108, 217, 148, 243, 64,
            84, 111, 240, 198, 115, 184, 214, 62, 101, 24, 68, 31, 221, 103, 16,
            241, 12, 25, 236, 174, 3, 161, 20, 123, 169, 11, 255, 248, 163, 192,
            162, 1, 247, 46, 188, 36, 104, 117, 13, 254, 186, 47, 181, 208, 218,
            61, 20, 83, 15, 86, 179, 200, 122, 156, 235, 101, 72, 23, 22, 21, 159,
            2, 204, 84, 124, 131, 0, 13, 12, 11, 162, 98, 168, 118, 219, 217, 237,
            199, 197, 164, 220, 172, 133, 116, 214, 208, 167, 155, 174, 154, 150,
            113, 102, 195, 99, 153, 184, 221, 115, 146, 142, 132, 125, 165, 94,
            209, 93, 147, 177, 87, 81, 80, 128, 137, 82, 148, 79, 78, 10, 107, 188,
            141, 127, 110, 71, 70, 65, 64, 68, 1, 17, 203, 3, 63, 247, 244, 225,
            169, 143, 60, 58, 249, 251, 240, 25, 48, 130, 9, 46, 201, 157, 160, 134,
            73, 238, 111, 77, 109, 196, 45, 129, 52, 37, 135, 27, 136, 170, 252,
            6, 161, 18, 56, 253, 76, 66, 114, 100, 19, 55, 36, 106, 117, 119, 67,
            255, 230, 180, 75, 54, 92, 228, 216, 53, 61, 69, 185, 44, 236, 183, 49,
            43, 41, 7, 104, 163, 14, 105, 123, 24, 158, 33, 57, 190, 40, 26, 91, 120,
            245, 35, 202, 42, 176, 175, 62, 254, 4, 140, 231, 229, 152, 50, 149, 211,
            246, 74, 232, 166, 234, 233, 243, 213, 47, 112, 32, 242, 31, 5, 103, 173,
            85, 16, 206, 205, 227, 39, 59, 218, 186, 215, 194, 38, 212, 145, 29, 210,
            28, 34, 51, 248, 250, 241, 90, 239, 207, 144, 182, 139, 181, 189, 192, 191,
            8, 151, 30, 108, 226, 97, 224, 198, 193, 89, 171, 187, 88, 222, 95, 223, 96,
            121, 126, 178, 138, 71, 241, 180, 230, 11, 106, 114, 72, 133, 78, 158, 235,
            226, 248, 148, 83, 224, 187, 160, 2, 232, 90, 9, 171, 219, 227, 186, 198,
            124, 195, 16, 221, 57, 5, 150, 48, 245, 55, 96, 130, 140, 201, 19, 74, 107,
            29, 243, 251, 143, 38, 151, 202, 145, 23, 1, 196, 50, 45, 110, 49, 149, 255,
            217, 35, 209, 0, 94, 121, 220, 68, 59, 26, 40, 197, 97, 87, 32, 144, 61,
            131, 185, 67, 190, 103, 210, 70, 66, 118, 192, 109, 91, 126, 178, 15, 22,
            41, 60, 169, 3, 84, 13, 218, 93, 223, 246, 183, 199, 98, 205, 141, 6, 211,
            105, 92, 134, 214, 20, 247, 165, 102, 117, 172, 177, 233, 69, 33, 112, 12,
            135, 159, 116, 164, 34, 76, 111, 191, 31, 86, 170, 46, 179, 120, 51, 80,
            176, 163, 146, 188, 207, 25, 28, 167, 99, 203, 30, 77, 62, 75, 27, 155, 79,
            231, 240, 238, 173, 58, 181, 89, 4, 234, 64, 85, 37, 81, 229, 122, 137, 56,
            104, 82, 123, 252, 39, 174, 215, 189, 250, 7, 244, 204, 142, 95, 239, 53, 156,
            132, 43, 21, 213, 119, 52, 73, 182, 18, 10, 127, 113, 136, 253, 157, 24, 65,
            125, 147, 216, 88, 44, 206, 254, 36, 175, 222, 184, 54, 200, 161, 128, 166,
            153, 152, 168, 47, 14, 129, 101, 115, 228, 194, 162, 138, 212, 225, 17, 208,
            8, 139, 42, 242, 237, 154, 100, 63, 193, 108, 249, 236
        };
    }
}

using pst.utilities;

namespace pst.encodables.ndb
{
    class Header
    {
        ///4
        public BinaryData Magic { get; }

        ///4
        public int CRCPartial { get; }

        ///2
        public BinaryData MagicClient { get; }

        ///2
        public int Version { get; }

        ///2
        public int ClientVersion { get; }

        ///1
        public int PlatformCreate { get; }

        ///1
        public int PlatformAccess { get; }

        ///4
        public BinaryData Reserved1 { get; }

        ///4
        public BinaryData Reserved2 { get; }

        ///8
        public BinaryData UnusedPadding { get; }

        ///8
        public BID NextPageBID { get; }

        ///4
        public int Unique { get; }

        ///128
        public NID[] NIDs { get; }

        ///8
        public BinaryData Unused { get; }

        //72
        public Root Root { get; }

        ///4
        public BinaryData AlignmentBytes { get; }

        ///128
        public BinaryData FMap { get; }

        ///128
        public BinaryData FPMap { get; }

        ///1
        public int Sentinel { get; }

        ///1
        public int CryptMethod { get; }

        ///2
        public BinaryData Reserved { get; }

        ///8
        public BID NextBID { get; }

        ///4
        public int CRCFull { get; }

        ///3
        public BinaryData RGBReserved2 { get; }

        ///1
        public BinaryData BReserved { get; }

        ///32
        public BinaryData RGBReserved3 { get; }

        public Header(
            BinaryData magic,
            int crcPartial,
            BinaryData magicClient,
            int version,
            int clientVersion,
            int platformCreate,
            int platformAccess,
            BinaryData reserved1,
            BinaryData reserved2,
            BinaryData unusedPadding,
            BID nextPageBID,
            int unique,
            NID[] nids,
            BinaryData unused,
            Root root,
            BinaryData alignmentBytes,
            BinaryData fmap,
            BinaryData fpmap,
            int sentinel,
            int cryptMethod,
            BinaryData reserved,
            BID nextBID,
            int crcFull,
            BinaryData rgbReserved2,
            BinaryData bReserved,
            BinaryData rgbReserved3)
        {
            Magic = magic;
            CRCPartial = crcPartial;
            MagicClient = magicClient;
            Version = version;
            ClientVersion = clientVersion;
            PlatformCreate = platformCreate;
            PlatformAccess = platformAccess;
            Reserved1 = reserved1;
            Reserved2 = reserved2;
            UnusedPadding = unusedPadding;
            NextPageBID = nextPageBID;
            Unique = unique;
            NIDs = nids;
            Unused = unused;
            Root = root;
            AlignmentBytes = alignmentBytes;
            FMap = fmap;
            FPMap = fpmap;
            Sentinel = sentinel;
            CryptMethod = cryptMethod;
            Reserved = reserved;
            NextBID = nextBID;
            CRCFull = crcFull;
            RGBReserved2 = rgbReserved2;
            BReserved = bReserved;
            RGBReserved3 = rgbReserved3;
        }
    }
}

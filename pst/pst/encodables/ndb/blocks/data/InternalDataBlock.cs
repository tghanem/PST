using pst.utilities;

namespace pst.encodables.ndb.blocks.data
{
    class InternalDataBlock
    {
        ///1
        public int BlockType { get; }

        ///1
        public int BlockLevel { get; }

        ///2
        public int NumberOfEntries { get; }

        ///4
        public int TotalByteCount { get; }

        ///NumberOfEntries * 16
        public BinaryData Entries { get; }

        ///(variable)
        public BinaryData Padding { get; }

        ///16
        public BlockTrailer Trailer { get; }

        public InternalDataBlock(int blockType, int blockLevel, int numberOfEntries, int totalByteCount, BinaryData entries, BinaryData padding, BlockTrailer trailer)
        {
            BlockType = blockType;
            BlockLevel = blockLevel;
            NumberOfEntries = numberOfEntries;
            TotalByteCount = totalByteCount;
            Entries = entries;
            Padding = padding;
            Trailer = trailer;
        }
    }
}

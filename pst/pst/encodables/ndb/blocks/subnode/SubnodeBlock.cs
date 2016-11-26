using pst.utilities;

namespace pst.encodables.ndb.blocks.subnode
{
    class SubnodeBlock
    {
        ///1
        public int BlockType { get; }

        ///1
        public int BlockLevel { get; }

        ///2
        public int NumberOfEntries { get; }

        ///4
        public BinaryData Padding { get; }

        ///(variable)
        public BinaryData Entries { get; }

        ///(variable)
        public BinaryData EntriesPadding { get; }

        ///16
        public BlockTrailer Trailer { get; }

        public SubnodeBlock(int blockType, int blockLevel, int numberOfEntries, BinaryData padding, BinaryData entries, BinaryData entriesPadding, BlockTrailer trailer)
        {
            BlockType = blockType;
            BlockLevel = blockLevel;
            NumberOfEntries = numberOfEntries;
            Padding = padding;
            Entries = entries;
            EntriesPadding = entriesPadding;
            Trailer = trailer;
        }
    }
}

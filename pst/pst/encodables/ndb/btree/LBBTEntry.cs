using pst.utilities;

namespace pst.encodables.ndb.btree
{
    class LBBTEntry
    {
        public BREF BlockReference { get; }

        public int ByteCountOfRawDataInReferencedBlockExcludingTrailerAndAlignmentPadding { get; }

        public int NumberOfReferencesToThisBlock { get; }

        public BinaryData Padding { get; }

        public LBBTEntry(BREF blockReference, int byteCountOfRawDataInReferencedBlockExcludingTrailerAndAlignmentPadding, int numberOfReferencesToThisBlock, BinaryData padding)
        {
            BlockReference = blockReference;
            ByteCountOfRawDataInReferencedBlockExcludingTrailerAndAlignmentPadding = byteCountOfRawDataInReferencedBlockExcludingTrailerAndAlignmentPadding;
            NumberOfReferencesToThisBlock = numberOfReferencesToThisBlock;
            Padding = padding;
        }

        public int GetBlockSize()
        {
            var rawDataSize =
                ByteCountOfRawDataInReferencedBlockExcludingTrailerAndAlignmentPadding;

            var paddingSize =
                (rawDataSize + 16).GetRemainingToNextMultipleOf(64);

            return rawDataSize + paddingSize + 16;
        }
    }
}

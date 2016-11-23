using pst.utilities;

namespace pst.encodables
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
    }
}

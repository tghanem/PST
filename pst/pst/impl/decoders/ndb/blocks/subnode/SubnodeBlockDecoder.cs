using pst.encodables.ndb.blocks.subnode;
using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb.blocks;

namespace pst.impl.decoders.ndb.blocks.subnode
{
    class SubnodeBlockDecoder : IDecoder<SubnodeBlock>
    {
        private readonly IDecoder<BlockTrailer> trailerDecoder;

        public SubnodeBlockDecoder(IDecoder<BlockTrailer> trailerDecoder)
        {
            this.trailerDecoder = trailerDecoder;
        }

        public SubnodeBlock Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            var blockType = parser.TakeAndSkip(1).ToInt32();
            var blockLevel = parser.TakeAndSkip(1).ToInt32();

            var entrySize =
                blockLevel == 1
                ? 16
                : 24;
            
            var numberOfEntries = parser.TakeAndSkip(2).ToInt32();
            var padding = parser.TakeAndSkip(4);
            var entries = parser.TakeAndSkip(numberOfEntries * entrySize);
            var entriesPadding = BinaryData.Empty();

            var remainingTo64Boundary =
                (numberOfEntries * entrySize + 8).GetRemainingToNextMultipleOf(64);

            if (remainingTo64Boundary > 0)
            {
                entriesPadding = parser.TakeAndSkip(remainingTo64Boundary);
            }

            var trailer = parser.TakeAndSkip(16, trailerDecoder);

            return
                new SubnodeBlock(
                    blockType,
                    blockLevel,
                    numberOfEntries,
                    padding,
                    entries,
                    entriesPadding,
                    trailer);
        }
    }
}

using pst.encodables.ndb.blocks.subnode;
using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb.blocks;

namespace pst.impl.decoders.ndb.blocks.subnode
{
    class SubnodeBlockDecoder : IDecoder<SubnodeBlock>
    {
        private readonly IDecoder<int> int32Decoder;

        private readonly IDecoder<BlockTrailer> trailerDecoder;

        private readonly int entrySize;

        public SubnodeBlockDecoder(IDecoder<int> int32Decoder, IDecoder<BlockTrailer> trailerDecoder, int entrySize)
        {
            this.int32Decoder = int32Decoder;
            this.trailerDecoder = trailerDecoder;
            this.entrySize = entrySize;
        }

        public SubnodeBlock Decode(BinaryData encodedData)
        {
            using (var parser = BinaryDataParser.OfValue(encodedData))
            {
                var blockType = parser.TakeAndSkip(1, int32Decoder);
                var blockLevel = parser.TakeAndSkip(1, int32Decoder); ;
                var numberOfEntries = parser.TakeAndSkip(2, int32Decoder);
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
}

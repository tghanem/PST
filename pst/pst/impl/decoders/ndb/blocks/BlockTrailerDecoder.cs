using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb;
using pst.encodables.ndb.blocks;

namespace pst.impl.decoders.ndb.blocks
{
    class BlockTrailerDecoder : IDecoder<BlockTrailer>
    {
        private readonly IDecoder<BID> bidDecoder;
        private readonly IDecoder<int> int32Decoder;

        public BlockTrailerDecoder(IDecoder<BID> bidDecoder, IDecoder<int> int32Decoder)
        {
            this.bidDecoder = bidDecoder;
            this.int32Decoder = int32Decoder;
        }

        public BlockTrailer Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new BlockTrailer(
                    parser.TakeAndSkip(2, int32Decoder),
                    parser.TakeAndSkip(2, int32Decoder),
                    parser.TakeAndSkip(4, int32Decoder),
                    parser.TakeAndSkip(8, bidDecoder));
        }
    }
}

using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb;
using pst.encodables.ndb.blocks;

namespace pst.impl.decoders.ndb.blocks
{
    class BlockTrailerDecoder : IDecoder<BlockTrailer>
    {
        private readonly IDecoder<BID> bidDecoder;

        public BlockTrailerDecoder(IDecoder<BID> bidDecoder)
        {
            this.bidDecoder = bidDecoder;
        }

        public BlockTrailer Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new BlockTrailer(
                    parser.TakeAndSkip(2).ToInt32(),
                    parser.TakeAndSkip(2).ToInt32(),
                    parser.TakeAndSkip(4).ToInt32(),
                    parser.TakeAndSkip(8, bidDecoder));
        }
    }
}

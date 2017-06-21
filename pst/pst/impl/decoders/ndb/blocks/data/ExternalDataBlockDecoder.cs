using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb.blocks;
using pst.encodables.ndb.blocks.data;

namespace pst.impl.decoders.ndb.blocks.data
{
    class ExternalDataBlockDecoder : IDecoder<ExternalDataBlock>
    {
        private readonly IDecoder<BlockTrailer> trailerDecoder;

        private readonly IBlockDataDeObfuscator blockDataDeObfuscator;

        public ExternalDataBlockDecoder(IDecoder<BlockTrailer> trailerDecoder, IBlockDataDeObfuscator blockDataDeObfuscator)
        {
            this.trailerDecoder = trailerDecoder;
            this.blockDataDeObfuscator = blockDataDeObfuscator;
        }

        public ExternalDataBlock Decode(BinaryData encodedData)
        {
            var parser =
                BinaryDataParser.OfValue(encodedData);

            var trailer =
                parser
                .TakeAtWithoutChangingStreamPosition(encodedData.Length - 16, 16, trailerDecoder);

            var encodedBlockData =
                parser.TakeAndSkip(trailer.AmountOfData);

            var decodedBlockData =
                blockDataDeObfuscator.DeObfuscate(encodedBlockData, trailer.BlockId);

            var padding =
                parser.TakeAndSkip(trailer.AmountOfData.GetRemainingToNextMultipleOf(64));

            return
                new ExternalDataBlock(decodedBlockData, padding, trailer);
        }
    }
}

using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb.blocks;
using pst.encodables.ndb.blocks.data;

namespace pst.impl.decoders.ndb.blocks.data
{
    class ExternalDataBlockDecoder : IDecoder<ExternalDataBlock>
    {
        private readonly IDecoder<BlockTrailer> trailerDecoder;

        private readonly IDecoder<BinaryData> blockDataDecoder;

        public ExternalDataBlockDecoder(IDecoder<BlockTrailer> trailerDecoder, IDecoder<BinaryData> blockDataDecoder)
        {
            this.trailerDecoder = trailerDecoder;
            this.blockDataDecoder = blockDataDecoder;
        }

        public ExternalDataBlock Decode(BinaryData encodedData)
        {
            using (var parser = BinaryDataParser.OfValue(encodedData))
            {
                var trailer =
                    parser
                    .TakeAtWithoutChangingStreamPosition(encodedData.Length - 16, 16, trailerDecoder);

                var encodedBlockData =
                    parser.TakeAndSkip(trailer.AmountOfData);

                var decodedBlockData =
                    blockDataDecoder.Decode(encodedBlockData);

                var padding =
                    parser.TakeAndSkip(trailer.AmountOfData.GetRemainingToNextMultipleOf(64));

                return
                    new ExternalDataBlock(
                        decodedBlockData,
                        padding,
                        trailer);
            }
        }
    }
}

using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb.blocks;
using pst.encodables.ndb.blocks.data;

namespace pst.impl.decoders.ndb.blocks.data
{
    class ExternalDataBlockDecoder : IDecoder<ExternalDataBlock>
    {
        private readonly IDecoder<BlockTrailer> trailerDecoder;

        public ExternalDataBlockDecoder(IDecoder<BlockTrailer> trailerDecoder)
        {
            this.trailerDecoder = trailerDecoder;
        }

        public ExternalDataBlock Decode(BinaryData encodedData)
        {
            using (var parser = BinaryDataParser.OfValue(encodedData))
            {
                var trailer =
                    parser
                    .TakeAtWithoutChangingStreamPosition(encodedData.Length - 16, 16, trailerDecoder);

                return
                    new ExternalDataBlock(
                        parser.TakeAndSkip(trailer.AmountOfData),
                        parser.TakeAndSkip(trailer.AmountOfData.GetRemainingToNextMultipleOf(64)),
                        trailer);
            }
        }
    }
}

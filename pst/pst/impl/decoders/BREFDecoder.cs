using pst.interfaces;
using pst.utilities;
using pst.encodables;

namespace pst.impl.decoders
{
    class BREFDecoder : IDecoder<BREF>
    {
        private readonly IDecoder<BID> bidDecoder;

        private readonly IDecoder<BI> biDecoder;

        public BREFDecoder(IDecoder<BID> bidDecoder, IDecoder<BI> biDecoder)
        {
            this.bidDecoder = bidDecoder;
            this.biDecoder = biDecoder;
        }

        public BREF Decode(BinaryData encodedData)
        {
            using (var parser = BinaryDataParser.OfValue(encodedData))
            {
                return
                    BREF.OfValue(
                        parser.TakeAndSkip(8, bidDecoder),
                        parser.TakeAndSkip(8, biDecoder));
            }
        }
    }
}

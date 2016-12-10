using pst.interfaces;
using pst.utilities;
using pst.encodables.ltp.tc;
using pst.encodables.ndb;

namespace pst.impl.decoders.ltp.tc
{
    class TCROWIDDecoder : IDecoder<TCROWID>
    {
        private readonly IDecoder<int> int32Decoder;
        private readonly IDecoder<NID> nidDecoder;

        public TCROWIDDecoder(IDecoder<int> int32Decoder, IDecoder<NID> nidDecoder)
        {
            this.int32Decoder = int32Decoder;
            this.nidDecoder = nidDecoder;
        }

        public TCROWID Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new TCROWID(
                    parser.TakeAndSkip(4, nidDecoder),
                    parser.TakeAndSkip(4, int32Decoder));
        }
    }
}

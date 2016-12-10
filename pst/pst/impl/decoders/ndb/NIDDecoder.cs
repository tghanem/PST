using pst.encodables.ndb;
using pst.interfaces;
using pst.utilities;
using System;

namespace pst.impl.decoders.ndb
{
    class NIDDecoder : IDecoder<NID>
    {
        private readonly IDecoder<int> int32Decoder;

        public NIDDecoder(IDecoder<int> int32Decoder)
        {
            this.int32Decoder = int32Decoder;
        }

        public NID Decode(BinaryData encodedData)
        {
            var value = int32Decoder.Decode(encodedData);

            var type = value & 0x0000001F;

            var index = Convert.ToInt32((value & 0xFFFFFFE0) >> 5);

            return new NID(type, index);
        }
    }
}

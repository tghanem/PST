using pst.encodables.ndb;
using pst.interfaces;
using pst.utilities;
using System;

namespace pst.impl.decoders.ndb
{
    class NIDDecoder : IDecoder<NID>
    {
        public NID Decode(BinaryData encodedData)
        {
            var value = BitConverter.ToInt32(encodedData.Value, 0);

            var type = value & 0x0000001F;

            var index = Convert.ToInt32((value & 0xFFFFFFE0) >> 5);

            return new NID(type, index);
        }
    }
}

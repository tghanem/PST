using pst.encodables.messaging.search;
using pst.encodables.ndb;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders.messaging.search
{
    class SUDMSGMovDecoder : IDecoder<SUDMSGMov>
    {
        private readonly IDecoder<NID> nidDecoder;

        public SUDMSGMovDecoder(IDecoder<NID> nidDecoder)
        {
            this.nidDecoder = nidDecoder;
        }

        public SUDMSGMov Decode(BinaryData encodedData)
        {
            return
                new SUDMSGMov(
                    nidDecoder.Decode(encodedData.Take(4)),
                    nidDecoder.Decode(encodedData.Take(4, 4)),
                    nidDecoder.Decode(encodedData.Take(8, 4)));
        }
    }
}

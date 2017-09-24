using pst.encodables.messaging.search;
using pst.encodables.ndb;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders.messaging.search
{
    class SUDMSGSpamDecoder : IDecoder<SUDMSGSpam>
    {
        private readonly IDecoder<NID> nidDecoder;

        public SUDMSGSpamDecoder(IDecoder<NID> nidDecoder)
        {
            this.nidDecoder = nidDecoder;
        }

        public SUDMSGSpam Decode(BinaryData encodedData)
        {
            return
                new SUDMSGSpam(
                    nidDecoder.Decode(encodedData.Take(4)),
                    nidDecoder.Decode(encodedData.Take(4, 4)));
        }
    }
}

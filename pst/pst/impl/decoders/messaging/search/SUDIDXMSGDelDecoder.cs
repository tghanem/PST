using pst.encodables.messaging.search;
using pst.encodables.ndb;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders.messaging.search
{
    class SUDIDXMSGDelDecoder : IDecoder<SUDIDXMSGDel>
    {
        private readonly IDecoder<NID> nidDecoder;

        public SUDIDXMSGDelDecoder(IDecoder<NID> nidDecoder)
        {
            this.nidDecoder = nidDecoder;
        }

        public SUDIDXMSGDel Decode(BinaryData encodedData)
        {
            return
                new SUDIDXMSGDel(
                    nidDecoder.Decode(encodedData.Take(4)),
                    nidDecoder.Decode(encodedData.Take(4, 4)));
        }
    }
}

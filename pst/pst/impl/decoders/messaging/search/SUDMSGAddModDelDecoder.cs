using pst.encodables.messaging.search;
using pst.encodables.ndb;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders.messaging.search
{
    class SUDMSGAddModDelDecoder : IDecoder<SUDMSGAddModDel>
    {
        private readonly IDecoder<NID> nidDecoder;

        public SUDMSGAddModDelDecoder(IDecoder<NID> nidDecoder)
        {
            this.nidDecoder = nidDecoder;
        }

        public SUDMSGAddModDel Decode(BinaryData encodedData)
        {
            return
                new SUDMSGAddModDel(
                    nidDecoder.Decode(encodedData.Take(4)),
                    nidDecoder.Decode(encodedData.Take(4, 4)));
        }
    }
}

using pst.encodables.messaging.search;
using pst.encodables.ndb;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders.messaging.search
{
    class SUDMSGIDXDecoder : IDecoder<SUDMSGIDX>
    {
        private readonly IDecoder<NID> nidDecoder;

        public SUDMSGIDXDecoder(IDecoder<NID> nidDecoder)
        {
            this.nidDecoder = nidDecoder;
        }

        public SUDMSGIDX Decode(BinaryData encodedData)
        {
            return 
                new SUDMSGIDX(
                    nidDecoder.Decode(encodedData.Take(4)));
        }
    }
}

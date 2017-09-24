using pst.encodables.messaging.search;
using pst.encodables.ndb;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders.messaging.search
{
    class SUDSRCHAddDelDecoder : IDecoder<SUDSRCHAddDel>
    {
        private readonly IDecoder<NID> nidDecoder;

        public SUDSRCHAddDelDecoder(IDecoder<NID> nidDecoder)
        {
            this.nidDecoder = nidDecoder;
        }

        public SUDSRCHAddDel Decode(BinaryData encodedData)
        {
            return
                new SUDSRCHAddDel(
                    nidDecoder.Decode(encodedData.Take(4)));
        }
    }
}

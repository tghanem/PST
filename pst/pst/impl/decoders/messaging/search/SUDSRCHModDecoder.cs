using pst.encodables.messaging.search;
using pst.encodables.ndb;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders.messaging.search
{
    class SUDSRCHModDecoder : IDecoder<SUDSRCHMod>
    {
        private readonly IDecoder<NID> nidDecoder;

        public SUDSRCHModDecoder(IDecoder<NID> nidDecoder)
        {
            this.nidDecoder = nidDecoder;
        }

        public SUDSRCHMod Decode(BinaryData encodedData)
        {
            return
                new SUDSRCHMod(
                    nidDecoder.Decode(encodedData.Take(4)),
                    encodedData.Take(4, 4).ToInt32());
        }
    }
}

using pst.encodables.messaging.search;
using pst.encodables.ndb;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders.messaging.search
{
    class SUDFLDAddMovDecoder : IDecoder<SUDFLDAddMov>
    {
        private readonly IDecoder<NID> nidDecoder;

        public SUDFLDAddMovDecoder(IDecoder<NID> nidDecoder)
        {
            this.nidDecoder = nidDecoder;
        }

        public SUDFLDAddMov Decode(BinaryData encodedData)
        {
            return
                new SUDFLDAddMov(
                    nidDecoder.Decode(encodedData.Take(4)),
                    nidDecoder.Decode(encodedData.Take(4, 4)),
                    encodedData.Take(8, 4).ToInt32(),
                    encodedData.Take(12, 4).ToInt32());
        }
    }
}

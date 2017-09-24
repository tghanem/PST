using pst.encodables.messaging.search;
using pst.encodables.ndb;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders.messaging.search
{
    class SUDFLDModDelDecoder : IDecoder<SUDFLDModDel>
    {
        private readonly IDecoder<NID> nidDecoder;

        public SUDFLDModDelDecoder(IDecoder<NID> nidDecoder)
        {
            this.nidDecoder = nidDecoder;
        }

        public SUDFLDModDel Decode(BinaryData encodedData)
        {
            return
                new SUDFLDModDel(
                    nidDecoder.Decode(encodedData.Take(4)),
                    encodedData.Take(4, 4).ToInt32());
        }
    }
}

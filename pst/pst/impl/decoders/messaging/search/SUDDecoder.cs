using pst.encodables.messaging.search;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders.messaging.search
{
    class SUDDecoder : IDecoder<SUD>
    {
        public SUD Decode(BinaryData encodedData)
        {
            return
                new SUD(
                    encodedData.Take(2).ToInt32(),
                    encodedData.Take(2, 2).ToInt32(),
                    encodedData.Take(4, 16));
        }
    }
}

using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb;

namespace pst.impl.encoders.ndb
{
    class NIDEncoder : IEncoder<NID>
    {
        public BinaryData Encode(NID value)
        {
            return BinaryData.From(value.Index | (value.Type << 5));
        }
    }
}

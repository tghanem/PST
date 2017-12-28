using pst.encodables.ndb;
using pst.interfaces;

namespace pst.utilities.encodings
{
    class NoEncoding : IEncoding
    {
        public BinaryData Obfuscate(BinaryData blockData, BID blockId)
        {
            return blockData;
        }

        public BinaryData DeObfuscate(BinaryData blockData, BID blockId)
        {
            return blockData;
        }
    }
}

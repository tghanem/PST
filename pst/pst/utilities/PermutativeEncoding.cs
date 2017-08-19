using pst.encodables.ndb;
using pst.interfaces;

namespace pst.utilities
{
    class PermutativeEncoding : IEncoding
    {
        public BinaryData Obfuscate(BinaryData blockData, BID blockId)
        {
            return Process(blockData, true);
        }

        public BinaryData DeObfuscate(BinaryData blockData, BID blockId)
        {
            return Process(blockData, false);
        }

        private BinaryData Process(BinaryData data, bool encrypt)
        {
            var decodedData = new byte[data.Length];

            var baseIndex = encrypt ? 0 : 512;

            for (var i = 0; i < data.Length; i++)
            {
                decodedData[i] = Globals.mpbbCrypt[baseIndex + data.Value[i]];
            }

            return BinaryData.OfValue(decodedData);
        }
    }
}

using pst.encodables.ndb;
using pst.interfaces;
using System.IO;

namespace pst.utilities
{
    class CyclicEncoding : IEncoding
    {
        public BinaryData Obfuscate(BinaryData blockData, BID blockId)
        {
            return Process(blockData, blockId);
        }

        public BinaryData DeObfuscate(BinaryData blockData, BID blockId)
        {
            return Process(blockData, blockId);
        }

        private BinaryData Process(BinaryData blockData, BID blockId)
        {
            var stream = new MemoryStream();

            var dwKey = (int)(blockId.Value & 0x0000FFFF);

            var w = (short)(dwKey ^ (dwKey >> 16));

            for (var i = 0; i < blockData.Length; i++)
            {
                var b = (byte)(blockData.Value[i] + (byte)w);
                b = Constants.mpbbCrypt[b];
                b = (byte)(b + (byte)(w >> 8));
                b = Constants.mpbbCrypt[b + 256];
                b = (byte)(b - (byte)(w >> 8));
                b = Constants.mpbbCrypt[b + 512];
                b = (byte)(b - (byte)w);

                stream.WriteByte(b);

                w++;
            }

            return BinaryData.OfValue(stream.ToArray());
        }
    }
}

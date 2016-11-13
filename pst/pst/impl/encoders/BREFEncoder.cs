using pst.interfaces;
using pst.utilities;
using pst.encodables;

namespace pst.impl.encoders
{
    class BREFEncoder : IEncoder<BREF>
    {
        private readonly IEncoder<BID> bidEncoder;

        private readonly IEncoder<BI> biEncoder;

        public BREFEncoder(IEncoder<BID> bidEncoder, IEncoder<BI> biEncoder)
        {
            this.bidEncoder = bidEncoder;
            this.biEncoder = biEncoder;
        }

        public BinaryData Encode(BREF value)
        {
            using (var generator = BinaryDataGenerator.New())
            {
                return
                    generator
                    .Append(value.BlockId, bidEncoder)
                    .Append(value.ByteIndex, biEncoder)
                    .GetData();
            }
        }
    }
}

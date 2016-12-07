using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb;

namespace pst.impl.encoders.ndb
{
    class BREFEncoder : IEncoder<BREF>
    {
        private readonly IEncoder<BID> bidEncoder;

        private readonly IEncoder<IB> biEncoder;

        public BREFEncoder(IEncoder<BID> bidEncoder, IEncoder<IB> biEncoder)
        {
            this.bidEncoder = bidEncoder;
            this.biEncoder = biEncoder;
        }

        public BinaryData Encode(BREF value)
        {
            var generator = BinaryDataGenerator.New();

            return
                generator
                .Append(value.BlockId, bidEncoder)
                .Append(value.ByteIndex, biEncoder)
                .GetData();
        }
    }
}

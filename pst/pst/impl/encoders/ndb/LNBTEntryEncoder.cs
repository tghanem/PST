using pst.encodables;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.encoders
{
    class LNBTEntryEncoder : IEncoder<LNBTEntry>
    {
        private readonly IEncoder<NID> nidEncoder;

        private readonly IEncoder<BID> bidEncoder;

        public LNBTEntryEncoder(IEncoder<NID> nidEncoder, IEncoder<BID> bidEncoder)
        {
            this.nidEncoder = nidEncoder;
            this.bidEncoder = bidEncoder;
        }

        public BinaryData Encode(LNBTEntry value)
        {
            using (var generator = BinaryDataGenerator.New())
            {
                return
                    generator
                    .Append(value.NodeId, nidEncoder)
                    .Append(value.DataBlockId, bidEncoder)
                    .Append(value.SubnodeBlockId, bidEncoder)
                    .Append(value.ParentNodeId, nidEncoder)
                    .Append(value.Padding)
                    .GetData();
            }
        }
    }
}

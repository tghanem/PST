using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;

namespace pst.impl.encoders.ndb.btree
{
    class LBBTEntryEncoder : IEncoder<LBBTEntry>
    {
        private readonly IEncoder<BREF> brefEncoder;

        private readonly IEncoder<int> int32Encoder;

        public LBBTEntryEncoder(IEncoder<BREF> brefEncoder, IEncoder<int> int32Encoder)
        {
            this.brefEncoder = brefEncoder;
            this.int32Encoder = int32Encoder;
        }

        public BinaryData Encode(LBBTEntry value)
        {
            using (var generator = BinaryDataGenerator.New())
            {
                return
                    generator
                    .Append(value.BlockReference, brefEncoder)
                    .Append(value.ByteCountOfRawDataInReferencedBlockExcludingTrailerAndAlignmentPadding, int32Encoder)
                    .Append(value.NumberOfReferencesToThisBlock, int32Encoder)
                    .Append(value.Padding)
                    .GetData();
            }
        }
    }
}

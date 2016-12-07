using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb.btree;
using pst.encodables.ndb;

namespace pst.impl.encoders.ndb.btree
{
    class IBBTEntryEncoder : IEncoder<IBBTEntry>
    {
        private readonly IEncoder<BID> bidEncoder;

        private readonly IEncoder<BREF> brefEncoder;

        public IBBTEntryEncoder(IEncoder<BID> bidEncoder, IEncoder<BREF> brefEncoder)
        {
            this.bidEncoder = bidEncoder;
            this.brefEncoder = brefEncoder;
        }

        public BinaryData Encode(IBBTEntry value)
        {
            var generator = BinaryDataGenerator.New();

            return
                generator
                .Append(value.Key, bidEncoder)
                .Append(value.ChildPageBlockReference, brefEncoder)
                .GetData();
        }
    }
}

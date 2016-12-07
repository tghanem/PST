using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb;

namespace pst.impl.encoders.ndb
{
    class RootEncoder : IEncoder<Root>
    {
        private readonly IEncoder<int> int32Encoder;

        private readonly IEncoder<long> int64Encoder;

        private readonly IEncoder<BREF> brefEncoder;

        public RootEncoder(IEncoder<int> int32Encoder, IEncoder<long> int64Encoder, IEncoder<BREF> brefEncoder)
        {
            this.int32Encoder = int32Encoder;
            this.int64Encoder = int64Encoder;
            this.brefEncoder = brefEncoder;
        }

        public BinaryData Encode(Root value)
        {
            var generator = BinaryDataGenerator.New();

            return
                generator
                .Append(value.Reserved, int32Encoder)
                .Append(value.FileEOF, int64Encoder)
                .Append(value.AMapLast, int64Encoder)
                .Append(value.AMapFree, int64Encoder)
                .Append(value.PMapFree, int64Encoder)
                .Append(value.NBTRootPage, brefEncoder)
                .Append(value.BBTRootPage, brefEncoder)
                .Append(value.AMapValid, int32Encoder, 1)
                .Append(value.BReserved, int32Encoder, 1)
                .Append(value.WReserved, int32Encoder, 2)
                .GetData();
        }
    }
}

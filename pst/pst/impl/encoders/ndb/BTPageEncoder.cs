using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb;

namespace pst.impl.encoders.ndb
{
    class BTPageEncoder : IEncoder<BTPage>
    {
        private readonly IEncoder<int> int32Encoder;
        private readonly IEncoder<PageTrailer> pageTrailerEncoder;

        public BTPageEncoder(IEncoder<int> int32Encoder, IEncoder<PageTrailer> pageTrailerEncoder)
        {
            this.int32Encoder = int32Encoder;
            this.pageTrailerEncoder = pageTrailerEncoder;
        }

        public BinaryData Encode(BTPage value)
        {
            using (var generator = BinaryDataGenerator.New())
            {
                return
                    generator
                    .Append(value.Entries)
                    .Append(value.Entries.Length, int32Encoder, 1)
                    .Append(value.MaximumNumberOfEntriesInPage, int32Encoder, 1)
                    .Append(value.EntrySize, int32Encoder, 1)
                    .Append(value.PageLevel, int32Encoder, 1)
                    .Append(value.PageTrailer, pageTrailerEncoder)
                    .GetData();
            }
        }
    }
}

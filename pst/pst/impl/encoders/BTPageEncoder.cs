using pst.interfaces;
using pst.utilities;
using pst.encodables;

namespace pst.impl.encoders
{
    class BTPageEncoder<TEntry> : IEncoder<BTPage<TEntry>>
    {
        private readonly IPageEntriesEncoder<TEntry> entriesEncoder;
        private readonly IEncoder<int> int32Encoder;
        private readonly IEncoder<PageTrailer> pageTrailerEncoder;

        public BTPageEncoder(IPageEntriesEncoder<TEntry> entriesEncoder, IEncoder<int> int32Encoder, IEncoder<PageTrailer> pageTrailerEncoder)
        {
            this.entriesEncoder = entriesEncoder;
            this.int32Encoder = int32Encoder;
            this.pageTrailerEncoder = pageTrailerEncoder;
        }

        public BinaryData Encode(BTPage<TEntry> value)
        {
            using (var generator = BinaryDataGenerator.New())
            {
                return
                    generator
                    .Append(entriesEncoder.Encode(value.PageTrailer.PageType, value.PageLevel, value.Entries))
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

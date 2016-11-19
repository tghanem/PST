using pst.interfaces;
using pst.utilities;
using pst.encodables;
using System;

namespace pst.impl.encoders
{
    class LNBTPageEncoder : IEncoder<LNBTPage>
    {
        private const int MaximumNumberOfEntriesInPage = 15;
        private const int EntrySize = 32;
        private const int PageLevel = 0;

        private readonly IEncoder<LNBTEntry> entryEncoder;

        private readonly IEncoder<int> int32Encoder;

        private readonly IEncoder<PageTrailer> pageTrailerEncoder;

        public LNBTPageEncoder(IEncoder<LNBTEntry> entryEncoder, IEncoder<int> int32Encoder, IEncoder<PageTrailer> pageTrailerEncoder)
        {
            this.entryEncoder = entryEncoder;
            this.int32Encoder = int32Encoder;
            this.pageTrailerEncoder = pageTrailerEncoder;
        }

        public BinaryData Encode(LNBTPage value)
        {
            using (var generator = BinaryDataGenerator.New())
            {
                Array.ForEach(value.Entries, e => generator.Append(e, entryEncoder));

                return
                    generator
                    .Append(value.Entries.Length, int32Encoder, 1)
                    .Append(MaximumNumberOfEntriesInPage, int32Encoder, 1)
                    .Append(EntrySize, int32Encoder, 1)
                    .Append(PageLevel, int32Encoder, 1)
                    .Append(value.Padding)
                    .Append(value.PageTrailer, pageTrailerEncoder)
                    .GetData();
            }
        }
    }
}

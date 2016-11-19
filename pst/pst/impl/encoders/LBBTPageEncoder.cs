using pst.interfaces;
using pst.utilities;
using pst.encodables;
using System;

namespace pst.impl.encoders
{
    class LBBTPageEncoder : IEncoder<LBBTPage>
    {
        private const int MaximumNumberOfEntriesInPage = 20;
        private const int EntrySize = 24;
        private const int PageLevel = 0;

        private readonly IEncoder<LBBTEntry> entryEncoder;

        private readonly IEncoder<int> int32Encoder;

        private readonly IEncoder<PageTrailer> pageTrailerEncoder;

        public LBBTPageEncoder(IEncoder<LBBTEntry> entryEncoder, IEncoder<int> int32Encoder, IEncoder<PageTrailer> pageTrailerEncoder)
        {
            this.entryEncoder = entryEncoder;
            this.int32Encoder = int32Encoder;
            this.pageTrailerEncoder = pageTrailerEncoder;
        }

        public BinaryData Encode(LBBTPage value)
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
                    .Append(value.PageTrailer, pageTrailerEncoder)
                    .GetData();
            }
        }
    }
}

using pst.interfaces;
using pst.utilities;

namespace pst.impl.encoders
{
    class BTPageEncoder : IEncoder<BTPage>
    {
        private readonly PSTFileEncoding encoding;

        public BTPageEncoder(PSTFileEncoding encoding)
        {
            this.encoding = encoding;
        }

        public BinaryData Encode(BTPage value)
        {
            if (encoding == PSTFileEncoding.ANSI)
            {
                value.RGEntries.AssertNotNullAndLength(496);

                value.PageTrailer.AssertNotNullAndLength(12);

                using (var generator = BinaryDataGenerator.New())
                {
                    return
                        generator
                        .Append(value.RGEntries)
                        .Append((byte)value.NumberOfEntriesInPage)
                        .Append((byte)value.MaximumNumberOfEntriesInPage)
                        .Append((byte)value.EntrySize)
                        .Append((byte)value.PageLevel)
                        .Append(value.PageTrailer)
                        .GetData();
                }
            }
            else
            {
                value.RGEntries.AssertNotNullAndLength(488);

                value.Padding.AssertNotNullAndLength(4);

                value.PageTrailer.AssertNotNullAndLength(16);

                using (var generator = BinaryDataGenerator.New())
                {
                    return
                        generator
                        .Append(value.RGEntries)
                        .Append((byte)value.NumberOfEntriesInPage)
                        .Append((byte)value.MaximumNumberOfEntriesInPage)
                        .Append((byte)value.EntrySize)
                        .Append((byte)value.PageLevel)
                        .Append(value.Padding)
                        .Append(value.PageTrailer)
                        .GetData();
                }
            }
        }
    }
}

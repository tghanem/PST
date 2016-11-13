using pst.interfaces;
using pst.utilities;
using System;

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
                if (value.RGEntries.Length > 496)
                    throw new ArgumentException("RGEntries length is more than expected");

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
                if (value.RGEntries.Length > 488)
                    throw new ArgumentException("RGEntries length is more than expected");

                if (value.Padding == null)
                    throw new ArgumentNullException("Padding is null");

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

using pst.interfaces;

namespace pst.impl.decoders
{
    public class BTPageDecoder : IDecoder<BTPage>
    {
        private readonly PSTFileEncoding encoding;

        public BTPageDecoder(PSTFileEncoding encoding)
        {
            this.encoding = encoding;
        }

        public BTPage Decode(BinaryData encodedData)
        {
            var rgEntries = (BinaryData) null;
            var numberOfEntriesInPage = 0;
            var maximumNumberOfEntriesInPage = 0;
            var entrySize = 0;
            var pageLevel = 0;
            var padding = (BinaryData) null;
            var pageTrailer = (BinaryData) null;

            if (encoding == PSTFileEncoding.ANSI)
            {
                encodedData
                    .TakeAndSkip(496, ref rgEntries)
                    .TakeAsInt32AndSkip(1, ref numberOfEntriesInPage)
                    .TakeAsInt32AndSkip(1, ref maximumNumberOfEntriesInPage)
                    .TakeAsInt32AndSkip(1, ref entrySize)
                    .TakeAsInt32AndSkip(1, ref pageLevel)
                    .TakeAndSkip(12, ref pageTrailer);
            }
            else
            {
                encodedData
                    .TakeAndSkip(488, ref rgEntries)
                    .TakeAsInt32AndSkip(1, ref numberOfEntriesInPage)
                    .TakeAsInt32AndSkip(1, ref maximumNumberOfEntriesInPage)
                    .TakeAsInt32AndSkip(1, ref entrySize)
                    .TakeAsInt32AndSkip(1, ref pageLevel)
                    .TakeAndSkip(4, ref padding)
                    .TakeAndSkip(16, ref pageTrailer);
            }

            return
                new BTPage(
                    rgEntries,
                    numberOfEntriesInPage,
                    maximumNumberOfEntriesInPage,
                    entrySize,
                    pageLevel,
                    padding,
                    pageTrailer);
        }
    }
}

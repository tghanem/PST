using pst.encodables;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders
{
    class BTPageDecoder<TEntry> : IDecoder<BTPage<TEntry>>
    {
        private readonly IPageEntriesDecoder<TEntry> entriesDecoder;

        private readonly IDecoder<int> int32Decoder;

        private readonly IDecoder<PageTrailer> pageTrailerDecoder;

        public BTPageDecoder(IPageEntriesDecoder<TEntry> entriesDecoder, IDecoder<int> int32Decoder, IDecoder<PageTrailer> pageTrailerDecoder)
        {
            this.entriesDecoder = entriesDecoder;
            this.int32Decoder = int32Decoder;
            this.pageTrailerDecoder = pageTrailerDecoder;
        }

        public BTPage<TEntry> Decode(BinaryData encodedData)
        {
            using (var parser = BinaryDataParser.OfValue(encodedData))
            {
                var encodedEntries = parser.TakeAndSkip(488);
                var numberOfEntriesInPage = parser.TakeAndSkip(1, int32Decoder);
                var maximumNumberOfEntries = parser.TakeAndSkip(1, int32Decoder);
                var entrySize = parser.TakeAndSkip(1, int32Decoder);
                var pageLevel = parser.TakeAndSkip(1, int32Decoder);
                var padding = parser.TakeAndSkip(4);
                var trailer = parser.TakeAndSkip(16, pageTrailerDecoder);

                return
                    new BTPage<TEntry>(
                        entriesDecoder.Decode(trailer.PageType, pageLevel, encodedEntries),
                        numberOfEntriesInPage,
                        maximumNumberOfEntries,
                        entrySize,
                        pageLevel,
                        padding,
                        trailer);
            }
        }
    }
}

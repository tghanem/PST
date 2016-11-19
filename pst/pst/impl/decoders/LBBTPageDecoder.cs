using pst.encodables;
using pst.interfaces;
using pst.utilities;
using System.Collections.Generic;

namespace pst.impl.decoders
{
    class LBBTPageDecoder : IDecoder<LBBTPage>
    {
        private readonly IDecoder<LBBTEntry> entryDecoder;

        private readonly IDecoder<int> int32Decoder;

        private readonly IDecoder<PageTrailer> pageTrailerDecoder;

        public LBBTPageDecoder(IDecoder<LBBTEntry> entryDecoder, IDecoder<int> int32Decoder, IDecoder<PageTrailer> pageTrailerDecoder)
        {
            this.entryDecoder = entryDecoder;
            this.int32Decoder = int32Decoder;
            this.pageTrailerDecoder = pageTrailerDecoder;
        }

        public LBBTPage Decode(BinaryData encodedData)
        {
            using (var parser = BinaryDataParser.OfValue(encodedData))
            {
                var numberOfEntries =
                    parser.TakeAtWithoutChangingStreamPosition(488, 1, int32Decoder);

                var entrySize =
                    parser.TakeAtWithoutChangingStreamPosition(490, 1, int32Decoder);

                var entries = new List<LBBTEntry>();

                for (var i = 0; i < numberOfEntries; i++)
                {
                    entries
                        .Add(parser.TakeAndSkip(entrySize, entryDecoder));
                }

                return
                    new LBBTPage(
                        entries.ToArray(),
                        parser.TakeAndSkip(1, int32Decoder),
                        parser.TakeAndSkip(1, int32Decoder),
                        parser.TakeAndSkip(1, int32Decoder),
                        parser.TakeAndSkip(1, int32Decoder),
                        parser.TakeAndSkip(4),
                        parser.TakeAndSkip(16, pageTrailerDecoder));
            }
        }
    }
}

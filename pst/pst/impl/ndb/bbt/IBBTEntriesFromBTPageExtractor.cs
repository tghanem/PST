using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.ndb.bbt
{
    class IBBTEntriesFromBTPageExtractor : IExtractor<BTPage, IBBTEntry[]>
    {
        private readonly IDecoder<IBBTEntry> entryDecoder;

        public IBBTEntriesFromBTPageExtractor(IDecoder<IBBTEntry> entryDecoder)
        {
            this.entryDecoder = entryDecoder;
        }

        public IBBTEntry[] Extract(BTPage parameter)
        {
            using (var parser = BinaryDataParser.OfValue(parameter.Entries))
            {
                return
                    parser
                    .TakeAndSkip(
                        parameter.NumberOfEntriesInPage,
                        parameter.EntrySize,
                        entryDecoder);
            }
        }
    }
}

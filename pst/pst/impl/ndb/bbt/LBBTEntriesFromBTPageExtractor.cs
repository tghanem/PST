using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.ndb.bbt
{
    class LBBTEntriesFromBTPageExtractor : IExtractor<BTPage, LBBTEntry[]>
    {
        private readonly IDecoder<LBBTEntry> entryDecoder;

        public LBBTEntriesFromBTPageExtractor(IDecoder<LBBTEntry> entryDecoder)
        {
            this.entryDecoder = entryDecoder;
        }

        public LBBTEntry[] Extract(BTPage parameter)
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

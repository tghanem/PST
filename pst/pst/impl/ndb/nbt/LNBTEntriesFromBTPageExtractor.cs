using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.ndb.nbt
{
    class LNBTEntriesFromBTPageExtractor : IExtractor<BTPage, LNBTEntry[]>
    {
        private readonly IDecoder<LNBTEntry> entryDecoder;

        public LNBTEntriesFromBTPageExtractor(IDecoder<LNBTEntry> entryDecoder)
        {
            this.entryDecoder = entryDecoder;
        }

        public LNBTEntry[] Extract(BTPage parameter)
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

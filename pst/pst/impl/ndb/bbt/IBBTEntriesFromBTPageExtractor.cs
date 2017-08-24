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
            return entryDecoder.DecodeMultipleItems(parameter.NumberOfEntriesInPage, parameter.EntrySize, parameter.Entries);
        }
    }
}

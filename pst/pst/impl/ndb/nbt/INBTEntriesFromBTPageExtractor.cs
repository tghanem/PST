using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.ndb.nbt
{
    class INBTEntriesFromBTPageExtractor : IExtractor<BTPage, INBTEntry[]>
    {
        private readonly IDecoder<INBTEntry> entryDecoder;

        public INBTEntriesFromBTPageExtractor(IDecoder<INBTEntry> entryDecoder)
        {
            this.entryDecoder = entryDecoder;
        }

        public INBTEntry[] Extract(BTPage parameter)
        {
            return entryDecoder.DecodeMultipleItems(parameter.NumberOfEntriesInPage, parameter.EntrySize, parameter.Entries);
        }
    }
}

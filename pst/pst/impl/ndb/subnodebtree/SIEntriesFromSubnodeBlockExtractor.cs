using pst.encodables.ndb.blocks.subnode;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.ndb.subnodebtree
{
    class SIEntriesFromSubnodeBlockExtractor : IExtractor<SubnodeBlock, SIEntry[]>
    {
        private readonly IDecoder<SIEntry> siEntryDecoder;

        public SIEntriesFromSubnodeBlockExtractor(IDecoder<SIEntry> siEntryDecoder)
        {
            this.siEntryDecoder = siEntryDecoder;
        }

        public SIEntry[] Extract(SubnodeBlock parameter)
        {
            return siEntryDecoder.DecodeMultipleItems(parameter.NumberOfEntries, 16, parameter.Entries);
        }
    }
}
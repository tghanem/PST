using pst.encodables.ndb.blocks.subnode;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.ndb.subnodebtree
{
    class SLEntriesFromSubnodeBlockExtractor : IExtractor<SubnodeBlock, SLEntry[]>
    {
        private readonly IDecoder<SLEntry> slEntryDecoder;

        public SLEntriesFromSubnodeBlockExtractor(IDecoder<SLEntry> slEntryDecoder)
        {
            this.slEntryDecoder = slEntryDecoder;
        }

        public SLEntry[] Extract(SubnodeBlock parameter)
        {
            return slEntryDecoder.DecodeMultipleItems(parameter.NumberOfEntries, 24, parameter.Entries);
        }
    }
}

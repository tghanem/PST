using pst.encodables;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.ltp;
using pst.interfaces.ltp.pc;
using pst.interfaces.ltp.tc;
using pst.interfaces.ndb;

namespace pst
{
    public partial class PSTFile
    {
        private readonly IDecoder<EntryId> entryIdDecoder;
        private readonly IDecoder<NID> nidDecoder;

        private readonly ITCReader<NID> tableContextReader;
        private readonly ITCReader<Tag> tagBasedTableContextReader;

        private readonly ISubNodesEnumerator subnodesEnumerator;
        private readonly IPropertyNameToIdMap propertyIdToNameMap;
        private readonly IPCBasedPropertyReader pcBasedPropertyReader;
        private readonly IMapper<NID, LNBTEntry> nidToLNBTEntryMapper;

        private PSTFile(
            ITCReader<NID> tableContextReader,
            ITCReader<Tag> tagBasedTableContextReader,
            IDecoder<EntryId> entryIdDecoder,
            IDecoder<NID> nidDecoder,
            ISubNodesEnumerator subnodesEnumerator,
            IPropertyNameToIdMap propertyIdToNameMap,
            IPCBasedPropertyReader pcBasedPropertyReader,
            IMapper<NID, LNBTEntry> nidToLNBTEntryMapper)
        {
            this.entryIdDecoder = entryIdDecoder;
            this.nidDecoder = nidDecoder;
            this.subnodesEnumerator = subnodesEnumerator;
            this.propertyIdToNameMap = propertyIdToNameMap;
            this.tableContextReader = tableContextReader;
            this.tagBasedTableContextReader = tagBasedTableContextReader;
            this.pcBasedPropertyReader = pcBasedPropertyReader;
            this.nidToLNBTEntryMapper = nidToLNBTEntryMapper;
        }

        public MessageStore MessageStore
            => new MessageStore(nidToLNBTEntryMapper, pcBasedPropertyReader);

        public Folder GetRootMailboxFolder()
        {
            var ipmSubtreeEntryId =
                MessageStore.GetProperty(MAPIProperties.PidTagIpmSubTreeEntryId);

            var entryId =
                entryIdDecoder.Decode(ipmSubtreeEntryId.Value.Value);

            return
                new Folder(
                    entryId.NID,
                    nidDecoder,
                    tableContextReader,
                    tagBasedTableContextReader,
                    subnodesEnumerator,
                    propertyIdToNameMap, 
                    pcBasedPropertyReader,
                    nidToLNBTEntryMapper);
        }
    }
}

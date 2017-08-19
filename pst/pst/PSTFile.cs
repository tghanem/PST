using pst.encodables;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.ltp;
using pst.interfaces.ltp.tc;
using pst.interfaces.ndb;

namespace pst
{
    public partial class PSTFile
    {
        private readonly IDecoder<EntryId> entryIdDecoder;
        private readonly IDecoder<NID> nidDecoder;

        private readonly IRowIndexReader<NID> rowIndexReader;
        private readonly ITableContextReader tableContextReader;
        private readonly ITableContextBasedPropertyReader<NID> nidBasedTableContextBasedPropertyReader;
        private readonly ITableContextBasedPropertyReader<Tag> tagBasedTableContextBasedPropertyReader;

        private readonly ISubNodesEnumerator subnodesEnumerator;
        private readonly IPropertyNameToIdMap propertyIdToNameMap;
        private readonly IPropertyReader propertyReader;
        private readonly IMapper<NID, LNBTEntry> nidToLNBTEntryMapper;

        private PSTFile(
            IRowIndexReader<NID> rowIndexReader,
            ITableContextReader tableContextReader,
            ITableContextBasedPropertyReader<NID> nidBasedTableContextBasedPropertyReader,
            ITableContextBasedPropertyReader<Tag> tagBasedTableContextBasedPropertyReader,
            IDecoder<EntryId> entryIdDecoder,
            IDecoder<NID> nidDecoder,
            ISubNodesEnumerator subnodesEnumerator,
            IPropertyNameToIdMap propertyIdToNameMap,
            IPropertyReader propertyReader,
            IMapper<NID, LNBTEntry> nidToLNBTEntryMapper)
        {
            this.rowIndexReader = rowIndexReader;
            this.tableContextReader = tableContextReader;
            this.entryIdDecoder = entryIdDecoder;
            this.nidDecoder = nidDecoder;
            this.subnodesEnumerator = subnodesEnumerator;
            this.propertyIdToNameMap = propertyIdToNameMap;
            this.nidBasedTableContextBasedPropertyReader = nidBasedTableContextBasedPropertyReader;
            this.tagBasedTableContextBasedPropertyReader = tagBasedTableContextBasedPropertyReader;
            this.propertyReader = propertyReader;
            this.nidToLNBTEntryMapper = nidToLNBTEntryMapper;
        }

        public MessageStore MessageStore
            => new MessageStore(nidToLNBTEntryMapper, propertyReader);

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
                    rowIndexReader,
                    tableContextReader, 
                    nidBasedTableContextBasedPropertyReader,
                    tagBasedTableContextBasedPropertyReader,
                    subnodesEnumerator,
                    propertyIdToNameMap, 
                    propertyReader,
                    nidToLNBTEntryMapper);
        }
    }
}

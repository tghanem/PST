using pst.encodables;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ltp.pc;
using pst.interfaces.ltp.tc;

namespace pst
{
    public partial class PSTFile
    {
        private readonly IPCBasedPropertyReader pcBasedPropertyReader;
        private readonly ITCReader<NID> tableContextReader;
        private readonly IDecoder<EntryId> entryIdDecoder;

        private PSTFile(
            IPCBasedPropertyReader pcBasedPropertyReader,
            ITCReader<NID> tableContextReader,
            IDecoder<EntryId> entryIdDecoder,
            MessageStore messageStore,
            Folder rootFolder)
        {
            this.pcBasedPropertyReader = pcBasedPropertyReader;
            this.tableContextReader = tableContextReader;
            this.entryIdDecoder = entryIdDecoder;

            MessageStore = messageStore;
            RootFolder = rootFolder;
        }

        public MessageStore MessageStore { get; }

        public Folder RootFolder { get; }

        public Folder GetRootMailboxFolder()
        {
            var ipmSubtreeEntryId =
                MessageStore.GetProperty(MAPIProperties.PidTagIpmSubTreeEntryId);

            var entryId =
                entryIdDecoder.Decode(ipmSubtreeEntryId.Value.Value);

            return
                new Folder(
                    entryId.NID,
                    pcBasedPropertyReader,
                    tableContextReader);
        }
    }
}

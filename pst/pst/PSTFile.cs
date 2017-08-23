using pst.encodables;
using pst.interfaces;
using pst.interfaces.messaging;

namespace pst
{
    public partial class PSTFile
    {
        private readonly IDecoder<EntryId> entryIdDecoder;
        private readonly IReadOnlyFolder readOnlyFolder;
        private readonly IReadOnlyMessage readOnlyMessage;
        private readonly IReadOnlyAttachment readOnlyAttachment;
        private readonly IPropertyContextBasedReadOnlyComponent propertyContextBasedReadOnlyComponent;
        private readonly ITableContextBasedReadOnlyComponent<Tag> readOnlyComponentForRecipient;

        internal PSTFile(
            IDecoder<EntryId> entryIdDecoder,
            IReadOnlyFolder readOnlyFolder,
            IReadOnlyMessage readOnlyMessage,
            IReadOnlyAttachment readOnlyAttachment,
            IPropertyContextBasedReadOnlyComponent propertyContextBasedReadOnlyComponent,
            ITableContextBasedReadOnlyComponent<Tag> readOnlyComponentForRecipient)
        {
            this.entryIdDecoder = entryIdDecoder;
            this.readOnlyFolder = readOnlyFolder;
            this.readOnlyMessage = readOnlyMessage;
            this.readOnlyAttachment = readOnlyAttachment;
            this.propertyContextBasedReadOnlyComponent = propertyContextBasedReadOnlyComponent;
            this.readOnlyComponentForRecipient = readOnlyComponentForRecipient;
        }

        public MessageStore MessageStore => new MessageStore(propertyContextBasedReadOnlyComponent);

        public Folder GetRootMailboxFolder()
        {
            var ipmSubtreeEntryId =
                MessageStore.GetProperty(MAPIProperties.PidTagIpmSubTreeEntryId);

            var entryId =
                entryIdDecoder.Decode(ipmSubtreeEntryId.Value.Value);

            return
                new Folder(
                    entryId.NID,
                    readOnlyFolder,
                    readOnlyMessage,
                    readOnlyAttachment,
                    propertyContextBasedReadOnlyComponent,
                    readOnlyComponentForRecipient);
        }
    }
}

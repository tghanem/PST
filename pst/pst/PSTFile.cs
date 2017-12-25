using pst.encodables;
using pst.interfaces;
using pst.interfaces.messaging;
using System;

namespace pst
{
    public partial class PSTFile
    {
        private readonly IDecoder<EntryId> entryIdDecoder;
        private readonly IFolder folder;
        private readonly IEncoder<string> stringEncoder;
        private readonly IReadOnlyMessage readOnlyMessage;
        private readonly IReadOnlyAttachment readOnlyAttachment;
        private readonly IPropertyContextBasedComponent propertyContextBasedComponent;
        private readonly ITableContextBasedReadOnlyComponent<Tag> readOnlyComponentForRecipient;

        private PSTFile(
            IDecoder<EntryId> entryIdDecoder,
            IFolder folder,
            IEncoder<string> stringEncoder,
            IReadOnlyMessage readOnlyMessage,
            IReadOnlyAttachment readOnlyAttachment,
            IPropertyContextBasedComponent propertyContextBasedComponent,
            ITableContextBasedReadOnlyComponent<Tag> readOnlyComponentForRecipient)
        {
            this.entryIdDecoder = entryIdDecoder;
            this.folder = folder;
            this.stringEncoder = stringEncoder;
            this.readOnlyMessage = readOnlyMessage;
            this.readOnlyAttachment = readOnlyAttachment;
            this.propertyContextBasedComponent = propertyContextBasedComponent;
            this.readOnlyComponentForRecipient = readOnlyComponentForRecipient;
        }

        public MessageStore MessageStore => new MessageStore(propertyContextBasedComponent);

        public Folder GetRootMailboxFolder()
        {
            var ipmSubtreeEntryId =
                MessageStore.GetProperty(MAPIProperties.PidTagIpmSubTreeEntryId);

            var entryId =
                entryIdDecoder.Decode(ipmSubtreeEntryId.Value.Value);

            return
                new Folder(
                    entryId.NID,
                    folder,
                    stringEncoder, 
                    readOnlyMessage,
                    readOnlyAttachment,
                    propertyContextBasedComponent,
                    readOnlyComponentForRecipient);
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}

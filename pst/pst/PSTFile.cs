using pst.encodables.ndb;
using pst.interfaces;
using pst.utilities;

namespace pst
{
    public partial class PSTFile
    {
        private readonly IFactory<NID, MessageStore> messageStoreFactory;
        private readonly IFactory<NID, Folder> folderFactory;

        private PSTFile(
            IFactory<NID, MessageStore> messageStoreFactory,
            IFactory<NID, Folder> folderFactory)
        {
            this.messageStoreFactory = messageStoreFactory;
            this.folderFactory = folderFactory;
        }

        public MessageStore GetMessageStore()
        {
            return messageStoreFactory.Create(Globals.NID_MESSAGE_STORE);
        }

        public Folder GetRootFolder()
        {
            return folderFactory.Create(new NID(0x122));
        }
    }
}

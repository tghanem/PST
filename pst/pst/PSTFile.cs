namespace pst
{
    public partial class PSTFile
    {
        public MessageStore MessageStore { get; }

        public Folder RootFolder { get; }

        private PSTFile(
            MessageStore messageStore,
            Folder rootFolder)
        {
            MessageStore = messageStore;
            RootFolder = rootFolder;
        }
    }
}

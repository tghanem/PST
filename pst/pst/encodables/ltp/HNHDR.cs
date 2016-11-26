namespace pst.encodables.ltp
{
    class HNHDR
    {
        ///2
        public int PageMapOffset { get; }

        ///1
        public int BlockSignature { get; }

        ///1
        public int ClientSignature { get; }

        ///4
        public int UserRoot { get; }

        ///4
        public int FillLevel { get; }

        public HNHDR(int pageMapOffset, int blockSignature, int clientSignature, int userRoot, int fillLevel)
        {
            PageMapOffset = pageMapOffset;
            BlockSignature = blockSignature;
            ClientSignature = clientSignature;
            UserRoot = userRoot;
            FillLevel = fillLevel;
        }
    }
}

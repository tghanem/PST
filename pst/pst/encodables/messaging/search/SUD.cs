using pst.utilities;

namespace pst.encodables.messaging.search
{
    class SUD
    {
        //2 bytes
        public int wFlags { get; }

        //2 bytes
        public int wSUDType { get; }

        //16 bytes
        public BinaryData SUDData { get; }

        public SUD(int wFlags, int wSudType, BinaryData sudData)
        {
            this.wFlags = wFlags;
            wSUDType = wSudType;
            SUDData = sudData;
        }
    }
}

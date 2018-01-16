using pst.interfaces;
using pst.utilities;
using pst.encodables.ltp.hn;
using pst.encodables.ndb;

namespace pst.impl.decoders.ltp.hn
{
    class HNIDDecoder : IDecoder<HNID>
    {
        private readonly IDecoder<HID> hidDecoder;

        public HNIDDecoder(IDecoder<HID> hidDecoder)
        {
            this.hidDecoder = hidDecoder;
        }

        public HNID Decode(BinaryData encodedData)
        {
            var hid = hidDecoder.Decode(encodedData);

            if (hid.Type == Constants.NID_TYPE_HID)
            {
                return new HNID(hid);
            }

            return new HNID(NID.OfValue(encodedData));
        }
    }
}

using pst.interfaces;
using pst.utilities;
using pst.encodables.ltp.hn;
using pst.encodables.ndb;

namespace pst.impl.decoders.ltp.hn
{
    class HNIDDecoder : IDecoder<HNID>
    {
        private readonly IDecoder<HID> hidDecoder;
        private readonly IDecoder<NID> nidDecoder;

        public HNIDDecoder(
            IDecoder<HID> hidDecoder,
            IDecoder<NID> nidDecoder)
        {
            this.hidDecoder = hidDecoder;
            this.nidDecoder = nidDecoder;
        }

        public HNID Decode(BinaryData encodedData)
        {
            var hid = hidDecoder.Decode(encodedData);

            if (hid.Type == Constants.NID_TYPE_HID)
            {
                return new HNID(hid, null);
            }

            return new HNID(null, nidDecoder.Decode(encodedData));
        }
    }
}

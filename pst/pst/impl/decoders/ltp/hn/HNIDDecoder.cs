using pst.encodables.ltp.hn;
using pst.encodables.ndb;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders.ltp.hn
{
    class HNIDDecoder : IDecoder<HNID>
    {
        public HNID Decode(BinaryData encodedData)
        {
            var hid = HID.OfValue(encodedData);

            if (hid.Type == Constants.NID_TYPE_HID)
            {
                return new HNID(hid);
            }

            return new HNID(NID.OfValue(encodedData));
        }
    }
}

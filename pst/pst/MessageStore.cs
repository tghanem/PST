using pst.encodables.ltp.bth;
using pst.encodables.ltp.hn;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.ltp.hn;
using pst.utilities;
using System.Text;

namespace pst
{
    public class MessageStore
    {
        private readonly IBTreeKeyFinder<DataRecord, PropertyId> propertyContext;
        private readonly IHeapOnNodeItemLoader heapOnNodeItemLoader;
        private readonly IDecoder<HID> hidDecoder;

        internal MessageStore(
            IBTreeKeyFinder<DataRecord, PropertyId> propertyContext,
            IHeapOnNodeItemLoader heapOnNodeItemLoader,
            IDecoder<HID> hidDecoder)
        {
            this.propertyContext = propertyContext;
            this.heapOnNodeItemLoader = heapOnNodeItemLoader;
            this.hidDecoder = hidDecoder;
        }

        public string DisplayName
        {
            get
            {
                var dataRecord =
                    propertyContext.Find(new PropertyId(0x3001));

                using (var parser = BinaryDataParser.OfValue(dataRecord.Value.Data))
                {
                    var propertyType = parser.TakeAndSkip(2);
                    var hid = parser.TakeAndSkip(4, hidDecoder);

                    var heapItem = heapOnNodeItemLoader.Load(hid);

                    return Encoding.Unicode.GetString(heapItem.Value.Value);
                }
            }
        }
    }
}

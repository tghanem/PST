using pst.encodables.ltp.bth;
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

        internal MessageStore(IBTreeKeyFinder<DataRecord, PropertyId> propertyContext, IHeapOnNodeItemLoader heapOnNodeItemLoader)
        {
            this.propertyContext = propertyContext;
            this.heapOnNodeItemLoader = heapOnNodeItemLoader;
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

                    var hid = parser.TakeAndSkip(4, Factory.HIDDecoder);

                    var heapItem = heapOnNodeItemLoader.Load(hid);

                    return Encoding.Unicode.GetString(heapItem.Value.Value);
                }
            }
        }
    }
}

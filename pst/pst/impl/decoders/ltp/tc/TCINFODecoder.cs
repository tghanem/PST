using pst.interfaces;
using pst.utilities;
using pst.encodables.ltp.tc;
using pst.encodables.ltp.hn;
using System.Linq;

namespace pst.impl.decoders.ltp.tc
{
    class TCINFODecoder : IDecoder<TCINFO>
    {
        private readonly IDecoder<HID> hidDecoder;
        private readonly IDecoder<TCOLDESC> columnDescriptorDecoder;

        public TCINFODecoder(
            IDecoder<HID> hidDecoder,
            IDecoder<TCOLDESC> columnDescriptorDecoder)
        {
            this.hidDecoder = hidDecoder;
            this.columnDescriptorDecoder = columnDescriptorDecoder;
        }

        public TCINFO Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            var type = parser.TakeAndSkip(1).ToInt32();
            var numberOfColumns = parser.TakeAndSkip(1).ToInt32();
            var groupsOffsets = parser.Slice(4, 2).Select(s => s.ToInt32()).ToArray();
            var rowIndex = parser.TakeAndSkip(4, hidDecoder);
            var rowMatrix = parser.TakeAndSkip(4);
            var deprecated = parser.TakeAndSkip(4);
            var columnDescriptors = parser.TakeAndSkip(numberOfColumns, 8, columnDescriptorDecoder);

            return
                new TCINFO(
                    type,
                    numberOfColumns,
                    groupsOffsets,
                    rowIndex,
                    rowMatrix,
                    deprecated,
                    columnDescriptors);
        }
    }
}

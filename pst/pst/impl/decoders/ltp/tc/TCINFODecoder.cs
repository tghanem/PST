using pst.encodables.ltp.hn;
using pst.encodables.ltp.tc;
using pst.interfaces;
using pst.utilities;
using System.Linq;

namespace pst.impl.decoders.ltp.tc
{
    class TCINFODecoder : IDecoder<TCINFO>
    {
        private readonly IDecoder<TCOLDESC> columnDescriptorDecoder;

        public TCINFODecoder(IDecoder<TCOLDESC> columnDescriptorDecoder)
        {
            this.columnDescriptorDecoder = columnDescriptorDecoder;
        }

        public TCINFO Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            var type = parser.TakeAndSkip(1).ToInt32();
            var numberOfColumns = parser.TakeAndSkip(1).ToInt32();
            var groupsOffsets = parser.Slice(4, 2).Select(s => s.ToInt32()).ToArray();
            var rowIndex = HID.OfValue(parser.TakeAndSkip(4));
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

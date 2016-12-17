using pst.interfaces;
using pst.utilities;
using pst.encodables.ltp.tc;
using pst.encodables.ltp.hn;

namespace pst.impl.decoders.ltp.tc
{
    class TCINFODecoder : IDecoder<TCINFO>
    {
        private readonly IDecoder<int> int32Decoder;
        private readonly IDecoder<HID> hidDecoder;
        private readonly IDecoder<TCOLDESC> columnDescriptorDecoder;

        public TCINFODecoder(
            IDecoder<int> int32Decoder,
            IDecoder<HID> hidDecoder,
            IDecoder<TCOLDESC> columnDescriptorDecoder)
        {
            this.int32Decoder = int32Decoder;
            this.hidDecoder = hidDecoder;
            this.columnDescriptorDecoder = columnDescriptorDecoder;
        }

        public TCINFO Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            var type = parser.TakeAndSkip(1, int32Decoder);
            var numberOfColumns = parser.TakeAndSkip(1, int32Decoder);
            var groupsOffsets = parser.TakeAndSkip(4, 2, int32Decoder);
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

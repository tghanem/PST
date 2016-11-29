using pst.encodables.ltp.bth;
using pst.interfaces;

namespace pst.impl.ltp.pc
{
    class PropertyIdFromIndexRecordExtractor : IExtractor<IndexRecord, PropertyId>
    {
        private readonly IDecoder<int> int32Decoder;

        public PropertyIdFromIndexRecordExtractor(IDecoder<int> int32Decoder)
        {
            this.int32Decoder = int32Decoder;
        }

        public PropertyId Extract(IndexRecord parameter)
        {
            return new PropertyId(int32Decoder.Decode(parameter.Key));
        }
    }
}

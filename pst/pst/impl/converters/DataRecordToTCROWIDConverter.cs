using pst.encodables.ltp.bth;
using pst.encodables.ltp.tc;
using pst.encodables.ndb;
using pst.interfaces;

namespace pst.impl.converters
{
    class DataRecordToTCROWIDConverter : IConverter<DataRecord, TCROWID>
    {
        private readonly IDecoder<NID> nidDecoder;
        private readonly IDecoder<int> int32Decoder;

        public DataRecordToTCROWIDConverter(
            IDecoder<NID> nidDecoder,
            IDecoder<int> int32Decoder)
        {
            this.nidDecoder = nidDecoder;
            this.int32Decoder = int32Decoder;
        }

        public TCROWID Convert(DataRecord parameter)
        {
            return
                new TCROWID(
                    nidDecoder.Decode(parameter.Key),
                    int32Decoder.Decode(parameter.Data));
        }
    }
}

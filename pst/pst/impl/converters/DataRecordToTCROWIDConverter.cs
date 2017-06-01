using pst.encodables.ltp.bth;
using pst.encodables.ltp.tc;
using pst.encodables.ndb;
using pst.interfaces;

namespace pst.impl.converters
{
    class DataRecordToTCROWIDConverter : IConverter<DataRecord, TCROWID>
    {
        private readonly IDecoder<NID> nidDecoder;

        public DataRecordToTCROWIDConverter(
            IDecoder<NID> nidDecoder)
        {
            this.nidDecoder = nidDecoder;
        }

        public TCROWID Convert(DataRecord parameter)
        {
            return
                new TCROWID(
                    nidDecoder.Decode(parameter.Key),
                    parameter.Data.ToInt32());
        }
    }
}

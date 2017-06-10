using pst.encodables.ltp.bth;
using pst.encodables.ltp.tc;
using pst.interfaces;

namespace pst.impl.converters
{
    class DataRecordToTCROWIDConverter : IConverter<DataRecord, TCROWID>
    {
        public TCROWID Convert(DataRecord parameter)
        {
            return
                new TCROWID(
                    parameter.Key,
                    parameter.Data.ToInt32());
        }
    }
}

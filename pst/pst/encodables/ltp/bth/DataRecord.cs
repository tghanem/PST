using pst.utilities;

namespace pst.encodables.ltp.bth
{
    class DataRecord
    {
        ///(variable)
        public BinaryData Key { get; }

        ///(variable)
        public BinaryData Data { get; }

        public DataRecord(BinaryData key, BinaryData data)
        {
            Key = key;
            Data = data;
        }
    }
}

using pst.encodables.ndb;
using pst.utilities;

namespace pst.interfaces.io
{
    interface IDataReader
    {
        BinaryData Read(IB byteIndex, int count);
    }

    interface IDataBlockReader<TBlockReference> where TBlockReference : class
    {
        BinaryData Read(TBlockReference blockReference, int blockSize);
    }
}

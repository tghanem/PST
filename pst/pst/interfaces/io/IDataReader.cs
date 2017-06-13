using pst.utilities;

namespace pst.interfaces.io
{
    interface IDataBlockReader<TBlockReference> where TBlockReference : class
    {
        BinaryData Read(TBlockReference blockReference, int blockSize);
    }
}

using pst.interfaces.io;
using pst.utilities;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;

namespace pst.impl.io
{
    class LBBTEntryBlockReaderAdapter
        : IDataBlockReader<LBBTEntry>
    {
        private readonly IDataBlockReader<BREF> actualReader;

        public LBBTEntryBlockReaderAdapter(
            IDataBlockReader<BREF> actualReader)
        {
            this.actualReader = actualReader;
        }

        public BinaryData Read(LBBTEntry blockReference, int blockSize)
        {
            return actualReader.Read(blockReference.BlockReference, blockSize);
        }
    }
}

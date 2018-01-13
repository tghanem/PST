using pst.encodables.ndb;
using pst.interfaces.io;
using pst.interfaces.ndb;
using pst.utilities;

namespace pst.impl.io
{
    class BlockIdBasedDataBlockReader : IDataBlockReader
    {
        private readonly IDataReader dataReader;
        private readonly IDataBlockEntryFinder dataBlockEntryFinder;

        public BlockIdBasedDataBlockReader(
            IDataReader dataReader,
            IDataBlockEntryFinder dataBlockEntryFinder)
        {
            this.dataReader = dataReader;
            this.dataBlockEntryFinder = dataBlockEntryFinder;
        }

        public BinaryData Read(BID blockId)
        {
            var dataBlockEntry = dataBlockEntryFinder.Find(blockId).Value;

            return
                dataReader.Read(
                    dataBlockEntry.BlockEntry.BlockReference.ByteIndex.Value,
                    dataBlockEntry.BlockEntry.ByteCountOfRawDataInReferencedBlockExcludingTrailerAndAlignmentPadding);
        }
    }
}

using pst.encodables.ndb.blocks.data;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.io;

namespace pst.impl.ndb.datatree
{
    class InternalDataBlockLoader :
        IBTreeNodeLoader<InternalDataBlock, LBBTEntry>
    {
        private readonly IDecoder<InternalDataBlock> internalDataBlockDecoder;

        private readonly IDataBlockReader<LBBTEntry> dataBlockReader;

        public InternalDataBlockLoader(
            IDecoder<InternalDataBlock> internalDataBlockDecoder,
            IDataBlockReader<LBBTEntry> dataBlockReader)
        {
            this.internalDataBlockDecoder = internalDataBlockDecoder;
            this.dataBlockReader = dataBlockReader;
        }

        public InternalDataBlock LoadNode(LBBTEntry nodeReference)
        {
            var encodedBlock =
                dataBlockReader
                .Read(
                    nodeReference,
                    nodeReference.GetBlockSize());

            return internalDataBlockDecoder.Decode(encodedBlock);
        }
    }
}

using pst.interfaces.btree;
using pst.core;
using pst.interfaces.io;
using pst.interfaces;
using pst.encodables.ndb.blocks.data;
using pst.encodables.ndb.btree;

namespace pst.impl.ndb.datatree
{
    class InternalDataBlockLoader :
        IBTreeNodeLoader<InternalDataBlock, LBBTEntry>
    {
        private readonly IDecoder<InternalDataBlock> internalDataBlockDecoder;

        public InternalDataBlockLoader(
            IDecoder<InternalDataBlock> internalDataBlockDecoder)
        {
            this.internalDataBlockDecoder = internalDataBlockDecoder;
        }

        public InternalDataBlock LoadNode(IDataBlockReader<LBBTEntry> reader, LBBTEntry nodeReference)
        {
            var encodedBlock =
                reader
                .Read(
                    nodeReference,
                    nodeReference.GetBlockSize());

            return internalDataBlockDecoder.Decode(encodedBlock);
        }
    }
}

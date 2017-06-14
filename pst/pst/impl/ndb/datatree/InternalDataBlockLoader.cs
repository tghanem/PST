using pst.encodables.ndb;
using pst.encodables.ndb.blocks.data;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.io;

namespace pst.impl.ndb.datatree
{
    class InternalDataBlockLoader : IBTreeNodeLoader<InternalDataBlock, BID>
    {
        private readonly IDecoder<InternalDataBlock> internalDataBlockDecoder;
        private readonly IDataBlockReader dataBlockReader;

        public InternalDataBlockLoader(IDecoder<InternalDataBlock> internalDataBlockDecoder, IDataBlockReader dataBlockReader)
        {
            this.internalDataBlockDecoder = internalDataBlockDecoder;
            this.dataBlockReader = dataBlockReader;
        }

        public InternalDataBlock LoadNode(BID nodeReference)
        {
            var encodedBlock = dataBlockReader.Read(nodeReference);

            return internalDataBlockDecoder.Decode(encodedBlock);
        }
    }
}

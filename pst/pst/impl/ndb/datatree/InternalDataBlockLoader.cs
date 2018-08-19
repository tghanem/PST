using pst.encodables.ndb;
using pst.encodables.ndb.blocks.data;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.io;
using pst.utilities;

namespace pst.impl.ndb.datatree
{
    class InternalDataBlockLoader : IBTreeNodeLoader<InternalDataBlock, LBBTEntry>
    {
        private readonly ICache<BID, InternalDataBlock> cache;
        private readonly IDataReader dataBlockReader;
        private readonly IDecoder<InternalDataBlock> internalDataBlockDecoder;

        public InternalDataBlockLoader(ICache<BID, InternalDataBlock> cache, IDataReader dataBlockReader, IDecoder<InternalDataBlock> internalDataBlockDecoder)
        {
            this.internalDataBlockDecoder = internalDataBlockDecoder;
            this.dataBlockReader = dataBlockReader;
            this.cache = cache;
        }

        public InternalDataBlock LoadNode(LBBTEntry nodeReference)
        {
            return
                cache
                .GetOrAdd(
                    nodeReference.BlockReference.BlockId,
                    () => internalDataBlockDecoder.Decode(dataBlockReader.Read(nodeReference.BlockReference.ByteIndex.Value, nodeReference.GetBlockSize())))
                .Value;
        }
    }
}

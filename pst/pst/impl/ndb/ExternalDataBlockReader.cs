using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ndb;
using pst.utilities;

namespace pst.impl.ndb
{
    class ExternalDataBlockReader : IExternalDataBlockReader
    {
        private readonly INodeEntryFinder nodeEntryFinder;
        private readonly IDataBlockEntryFinder dataBlockEntryFinder;
        private readonly IDataBlockReader dataBlockReader;
        private readonly IBlockDataDeObfuscator blockDataDeObfuscator;

        public ExternalDataBlockReader(
            INodeEntryFinder nodeEntryFinder,
            IDataBlockEntryFinder dataBlockEntryFinder,
            IDataBlockReader dataBlockReader,
            IBlockDataDeObfuscator blockDataDeObfuscator)
        {
            this.nodeEntryFinder = nodeEntryFinder;
            this.dataBlockEntryFinder = dataBlockEntryFinder;
            this.dataBlockReader = dataBlockReader;
            this.blockDataDeObfuscator = blockDataDeObfuscator;
        }

        public BinaryData Read(NodePath nodePath, int blockIndex)
        {
            var nodeEntry = nodeEntryFinder.GetEntry(nodePath);

            return ReadExternalDataBlock(nodeEntry.Value.NodeDataBlockId, blockIndex);
        }

        private BinaryData ReadExternalDataBlock(BID blockId, int blockIndex)
        {
            var dataBlockTree = dataBlockEntryFinder.Find(blockId);

            var actualBlockId = blockId;

            if (dataBlockTree.Value.ChildBlockIds.HasValueAnd(childBlockIds => childBlockIds.Length > 0))
            {
                actualBlockId = dataBlockTree.Value.ChildBlockIds.Value[blockIndex];
            }

            var externalDataBlock = dataBlockReader.Read(actualBlockId);

            return blockDataDeObfuscator.DeObfuscate(externalDataBlock, actualBlockId);
        }
    }
}

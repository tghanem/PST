using pst.core;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ndb;
using pst.utilities;
using System.Linq;

namespace pst.impl.ndb
{
    class DataTreeReader : IDataTreeReader
    {
        private readonly INodeEntryFinder nodeEntryFinder;
        private readonly IDataBlockEntryFinder dataBlockEntryFinder;
        private readonly IDataBlockReader dataBlockReader;
        private readonly IBlockDataDeObfuscator blockDataDeObfuscator;

        public DataTreeReader(
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

        public BinaryData[] Read(NID[] nodePath, Maybe<int> blockIndex)
        {
            var nodeEntry = nodeEntryFinder.GetEntry(nodePath);

            return Read(nodeEntry.Value.NodeDataBlockId, blockIndex);
        }

        private BinaryData[] Read(BID dataTreeRootBlockId, Maybe<int> blockIndex)
        {
            var dataBlockTree = dataBlockEntryFinder.Find(dataTreeRootBlockId);

            if (blockIndex.HasNoValue)
            {
                return dataBlockTree.Value.ChildBlockIds.Value.Select(ReadExternalBlock).ToArray();
            }

            if (dataBlockTree.Value.ChildBlockIds.HasValueAnd(childBlockIds => childBlockIds.Length > 0))
            {
                var leafBlockId = dataBlockTree.Value.ChildBlockIds.Value[blockIndex.Value];

                return new[] { ReadExternalBlock(leafBlockId) };
            }

            return new BinaryData[0];
        }

        private BinaryData ReadExternalBlock(BID blockId)
        {
            var externalDataBlock = dataBlockReader.Read(blockId);

            return blockDataDeObfuscator.DeObfuscate(externalDataBlock, blockId);
        }
    }
}

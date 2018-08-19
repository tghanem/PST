using pst.core;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.ndb;
using pst.utilities;
using System.Linq;

namespace pst.impl.ndb
{
    class DataTreeReader : IDataTreeReader
    {
        private readonly INodeEntryFinder nodeEntryFinder;
        private readonly IDataBlockReader dataBlockReader;
        private readonly IBlockDataDeObfuscator blockDataDeObfuscator;
        private readonly IHeaderReader headerReader;
        private readonly IBTreeEntryFinder<BID, LBBTEntry, BREF> blockBTreeEntryFinder;
        private readonly IExternalDataBlockIdsReader externalDataBlockIdsReader;

        public DataTreeReader(
            INodeEntryFinder nodeEntryFinder,
            IDataBlockReader dataBlockReader,
            IBlockDataDeObfuscator blockDataDeObfuscator,
            IHeaderReader headerReader,
            IBTreeEntryFinder<BID, LBBTEntry, BREF> blockBTreeEntryFinder,
            IExternalDataBlockIdsReader externalDataBlockIdsReader)
        {
            this.nodeEntryFinder = nodeEntryFinder;
            this.dataBlockReader = dataBlockReader;
            this.blockDataDeObfuscator = blockDataDeObfuscator;
            this.headerReader = headerReader;
            this.blockBTreeEntryFinder = blockBTreeEntryFinder;
            this.externalDataBlockIdsReader = externalDataBlockIdsReader;
        }

        public BinaryData[] Read(NID[] nodePath, Maybe<int> blockIndex)
        {
            var nodeEntry = nodeEntryFinder.GetEntry(nodePath);

            return Read(nodeEntry.Value.NodeDataBlockId, blockIndex);
        }

        private BinaryData[] Read(BID dataTreeRootBlockId, Maybe<int> blockIndex)
        {
            var header = headerReader.GetHeader();

            var lbbtEntry = blockBTreeEntryFinder.Find(dataTreeRootBlockId, header.Root.BBTRootPage);

            var externalDataBlockIds = externalDataBlockIdsReader.Read(lbbtEntry.Value);

            if (blockIndex.HasNoValue)
            {
                return externalDataBlockIds.Select(ReadExternalBlock).ToArray();
            }

            if (externalDataBlockIds.Length > 0)
            {
                var leafBlockId = externalDataBlockIds[blockIndex.Value];

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

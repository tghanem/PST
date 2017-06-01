using pst.encodables.ndb;
using pst.encodables.ndb.blocks.data;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.io;
using pst.interfaces.ndb;
using System.Collections.Generic;

namespace pst.impl.ndb.datatree
{
    class DataTreeLeafNodesEnumerator : IDataTreeLeafNodesEnumerator
    {
        private readonly IBTreeLeafKeysEnumeratorThatDoesntKnowHowToMapTheKeyToNodeReference<BID, BID, LBBTEntry> dataTreeLeafKeysEnumerator;
        private readonly IDecoder<ExternalDataBlock> externalDataBlockDecoder;

        public DataTreeLeafNodesEnumerator(
            IBTreeLeafKeysEnumeratorThatDoesntKnowHowToMapTheKeyToNodeReference<BID, BID, LBBTEntry> dataTreeLeafKeysEnumerator,
            IDecoder<ExternalDataBlock> externalDataBlockDecoder)
        {
            this.dataTreeLeafKeysEnumerator = dataTreeLeafKeysEnumerator;
            this.externalDataBlockDecoder = externalDataBlockDecoder;
        }

        public BID[] Enumerate(
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry)
        {
            var blockSize = blockEntry.GetBlockSize();

            if (blockSize <= 8 * 1024)
            {
                return new[] { blockEntry.BlockReference.BlockId };
            }
            else
            {
                var blockIds = new List<BID>();

                var bids =
                    dataTreeLeafKeysEnumerator
                    .Enumerate(
                        blockIdToEntryMapping,
                        blockEntry);

                foreach (var bid in bids)
                {
                    blockIds.Add(bid);
                }

                return blockIds.ToArray();
            }
        }

        private ExternalDataBlock LoadBlock(IDataBlockReader<LBBTEntry> reader, LBBTEntry blockEntry)
        {
            var blockSize = blockEntry.GetBlockSize();

            var encodedDataBlock = reader.Read(blockEntry, blockSize);

            return externalDataBlockDecoder.Decode(encodedDataBlock);
        }
    }
}

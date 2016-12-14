using pst.interfaces.ndb;
using System.Collections.Generic;
using pst.encodables.ndb;
using pst.encodables.ndb.blocks.data;
using pst.encodables.ndb.btree;
using pst.interfaces.io;
using pst.interfaces.btree;
using pst.interfaces;

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

        public ExternalDataBlock[] Enumerate(
            IDataBlockReader<LBBTEntry> reader,
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry)
        {
            var blockSize = blockEntry.GetBlockSize();

            if (blockSize <= 8*1024)
            {
                return new[] {LoadBlock(reader, blockEntry)};
            }
            else
            {
                var blocks = new List<ExternalDataBlock>();

                var bids =
                    dataTreeLeafKeysEnumerator
                    .Enumerate(
                        reader,
                        blockIdToEntryMapping,
                        blockEntry);

                foreach (var bid in bids)
                {
                    var entry = blockIdToEntryMapping.Map(bid);

                    var block = LoadBlock(reader, entry);

                    blocks.Add(block);
                }

                return blocks.ToArray();
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

using pst.encodables.ndb;
using pst.encodables.ndb.blocks.data;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.ndb;
using System.Collections.Generic;

namespace pst.impl.ndb.datatree
{
    class DataTreeLeafBIDsEnumerator : IDataTreeLeafBIDsEnumerator
    {
        private readonly IBTreeLeafKeysEnumeratorThatDoesntKnowHowToMapTheKeyToNodeReference<BID, BID, LBBTEntry> dataTreeLeafKeysEnumerator;
        private readonly IDecoder<ExternalDataBlock> externalDataBlockDecoder;

        private readonly IMapper<BID, LBBTEntry> bidToLBBTEntryMapper;

        public DataTreeLeafBIDsEnumerator(
            IBTreeLeafKeysEnumeratorThatDoesntKnowHowToMapTheKeyToNodeReference<BID, BID, LBBTEntry> dataTreeLeafKeysEnumerator,
            IDecoder<ExternalDataBlock> externalDataBlockDecoder,
            IMapper<BID, LBBTEntry> bidToLBBTEntryMapper)
        {
            this.dataTreeLeafKeysEnumerator = dataTreeLeafKeysEnumerator;
            this.externalDataBlockDecoder = externalDataBlockDecoder;
            this.bidToLBBTEntryMapper = bidToLBBTEntryMapper;
        }

        public BID[] Enumerate(BID blockId)
        {
            var lbbtEntry = bidToLBBTEntryMapper.Map(blockId);

            var blockSize = lbbtEntry.GetBlockSize();

            if (blockSize <= 8 * 1024)
            {
                return new[] { lbbtEntry.BlockReference.BlockId };
            }
            else
            {
                var blockIds = new List<BID>();

                var bids = dataTreeLeafKeysEnumerator.Enumerate(lbbtEntry);

                foreach (var bid in bids)
                {
                    blockIds.Add(bid);
                }

                return blockIds.ToArray();
            }
        }
    }
}

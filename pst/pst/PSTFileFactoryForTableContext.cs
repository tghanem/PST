using pst.encodables.ndb;
using pst.encodables.ndb.blocks.data;
using pst.encodables.ndb.blocks.subnode;
using pst.encodables.ndb.btree;
using pst.impl.ltp.tc;
using pst.interfaces;
using pst.interfaces.ltp.tc;
using System.IO;

namespace pst
{
    public partial class PSTFile
    {
        private static IRowIndexReader CreateRowIndexReader(
            Stream dataStream,
            ICache<BID, BTPage> cachedBTPages,
            ICache<BID, SubnodeBlock> cachedSubnodeBlocks,
            ICache<BID, InternalDataBlock> cachedInternalDataBlocks,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new RowIndexReader(
                    CreateHeapOnNodeReader(dataStream, cachedBTPages, cachedSubnodeBlocks, cachedInternalDataBlocks, cachedHeaderHolder), 
                    CreateInt32BasedBTreeOnHeapReader(dataStream, cachedBTPages, cachedSubnodeBlocks, cachedInternalDataBlocks, cachedHeaderHolder));
        }

        private static IRowMatrixReader CreateRowMatrixReader(
            Stream dataStream,
            ICache<BID, BTPage> cachedBTPages,
            ICache<BID, SubnodeBlock> cachedSubnodeBlocks,
            ICache<BID, InternalDataBlock> cachedInternalDataBlocks,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new RowMatrixReader(
                    CreateHeapOnNodeReader(dataStream, cachedBTPages, cachedSubnodeBlocks, cachedInternalDataBlocks, cachedHeaderHolder), 
                    new RowValuesExtractor(),
                    CreateDataTreeReader(dataStream, cachedBTPages, cachedInternalDataBlocks, cachedSubnodeBlocks, cachedHeaderHolder));
        }
    }
}

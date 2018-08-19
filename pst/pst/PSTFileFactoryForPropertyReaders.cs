using pst.encodables.ndb;
using pst.encodables.ndb.blocks.data;
using pst.encodables.ndb.blocks.subnode;
using pst.encodables.ndb.btree;
using pst.impl.messaging;
using pst.interfaces;
using pst.interfaces.ltp;
using pst.interfaces.messaging;
using System.IO;

namespace pst
{
    public partial class PSTFile
    {
        private static ITableContextBasedPropertyReader CreateTagBasedTableContextBasedPropertyReader(
            Stream dataStream,
            ICache<BID, BTPage> cachedBTPages,
            ICache<BID, SubnodeBlock> cachedSubnodeBlocks,
            ICache<BID, InternalDataBlock> cachedInternalDataBlocks,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new TableContextBasedPropertyReader(
                    CreateRowIndexReader(dataStream, cachedBTPages, cachedSubnodeBlocks, cachedInternalDataBlocks, cachedHeaderHolder), 
                    CreateRowMatrixReader(dataStream, cachedBTPages, cachedSubnodeBlocks, cachedInternalDataBlocks, cachedHeaderHolder), 
                    CreatePropertyValueProcessor(dataStream, cachedBTPages, cachedSubnodeBlocks, cachedInternalDataBlocks, cachedHeaderHolder));
        }

        private static IPropertyNameToIdMap CreatePropertyIdToNameMap(
            Stream dataStream,
            ICache<BID, BTPage> cachedBTPages,
            ICache<BID, SubnodeBlock> cachedSubnodeBlocks,
            ICache<BID, InternalDataBlock> cachedInternalDataBlocks,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new PropertyNameToIdMap(
                    CreatePropertyContextBasedPropertyReader(dataStream, cachedBTPages, cachedSubnodeBlocks, cachedInternalDataBlocks, cachedHeaderHolder));
        }

        private static IPropertyContextBasedPropertyReader CreatePropertyContextBasedPropertyReader(
            Stream dataStream,
            ICache<BID, BTPage> cachedBTPages,
            ICache<BID, SubnodeBlock> cachedSubnodeBlocks,
            ICache<BID, InternalDataBlock> cachedInternalDataBlocks,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new PropertyContextBasedPropertyReader(
                    CreatePropertyIdBasedBTreeOnHeapReader(dataStream, cachedBTPages, cachedSubnodeBlocks, cachedInternalDataBlocks, cachedHeaderHolder), 
                    CreatePropertyValueProcessor(dataStream, cachedBTPages, cachedSubnodeBlocks, cachedInternalDataBlocks, cachedHeaderHolder));
        }

        private static IPropertyValueReader CreatePropertyValueProcessor(
            Stream dataStream,
            ICache<BID, BTPage> cachedBTPages,
            ICache<BID, SubnodeBlock> cachedSubnodeBlocks,
            ICache<BID, InternalDataBlock> cachedInternalDataBlocks,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new PropertyValueReader(
                    CreateHeapOnNodeReader(dataStream, cachedBTPages, cachedSubnodeBlocks, cachedInternalDataBlocks, cachedHeaderHolder), 
                    CreateDataTreeReader(dataStream, cachedBTPages, cachedInternalDataBlocks, cachedSubnodeBlocks, cachedHeaderHolder));
        }
    }
}

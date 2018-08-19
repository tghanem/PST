﻿using pst.encodables.ndb;
using pst.encodables.ndb.blocks.data;
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
            ICache<BID, InternalDataBlock> cachedInternalDataBlocks,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new TableContextBasedPropertyReader(
                    CreateRowIndexReader(dataStream, cachedBTPages, cachedInternalDataBlocks, cachedHeaderHolder), 
                    CreateRowMatrixReader(dataStream, cachedBTPages, cachedInternalDataBlocks, cachedHeaderHolder), 
                    CreatePropertyValueProcessor(dataStream, cachedBTPages, cachedInternalDataBlocks, cachedHeaderHolder));
        }

        private static IPropertyNameToIdMap CreatePropertyIdToNameMap(
            Stream dataStream,
            ICache<BID, BTPage> cachedBTPages,
            ICache<BID, InternalDataBlock> cachedInternalDataBlocks,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new PropertyNameToIdMap(
                    CreatePropertyContextBasedPropertyReader(dataStream, cachedBTPages, cachedInternalDataBlocks, cachedHeaderHolder));
        }

        private static IPropertyContextBasedPropertyReader CreatePropertyContextBasedPropertyReader(
            Stream dataStream,
            ICache<BID, BTPage> cachedBTPages,
            ICache<BID, InternalDataBlock> cachedInternalDataBlocks,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new PropertyContextBasedPropertyReader(
                    CreatePropertyIdBasedBTreeOnHeapReader(dataStream, cachedBTPages, cachedInternalDataBlocks, cachedHeaderHolder), 
                    CreatePropertyValueProcessor(dataStream, cachedBTPages, cachedInternalDataBlocks, cachedHeaderHolder));
        }

        private static IPropertyValueReader CreatePropertyValueProcessor(
            Stream dataStream,
            ICache<BID, BTPage> cachedBTPages,
            ICache<BID, InternalDataBlock> cachedInternalDataBlocks,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new PropertyValueReader(
                    CreateHeapOnNodeReader(dataStream, cachedBTPages, cachedInternalDataBlocks, cachedHeaderHolder), 
                    CreateDataTreeReader(dataStream, cachedBTPages, cachedInternalDataBlocks, cachedHeaderHolder));
        }
    }
}

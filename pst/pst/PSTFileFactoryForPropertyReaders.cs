using pst.encodables.ndb;
using pst.impl.messaging;
using pst.interfaces;
using pst.interfaces.ltp;
using pst.interfaces.messaging;
using pst.interfaces.ndb;
using System.IO;

namespace pst
{
    public partial class PSTFile
    {
        private static ITableContextBasedPropertyReader CreateTagBasedTableContextBasedPropertyReader(
            Stream dataStream,
            ICache<NID[], NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new TableContextBasedPropertyReader(
                    CreateRowIndexReader(dataStream, nodeEntryCache, dataBlockEntryCache, cachedHeaderHolder), 
                    CreateRowMatrixReader(dataStream, nodeEntryCache, dataBlockEntryCache, cachedHeaderHolder),
                    CreatePropertyValueProcessor(dataStream, nodeEntryCache, dataBlockEntryCache, cachedHeaderHolder));
        }

        private static IPropertyNameToIdMap CreatePropertyIdToNameMap(
            Stream dataStream,
            ICache<NID[], NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return new PropertyNameToIdMap(CreatePropertyContextBasedPropertyReader(dataStream, nodeEntryCache, dataBlockEntryCache, cachedHeaderHolder));
        }

        private static IPropertyContextBasedPropertyReader CreatePropertyContextBasedPropertyReader(
            Stream dataStream,
            ICache<NID[], NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new PropertyContextBasedPropertyReader(
                    CreatePropertyIdBasedBTreeOnHeapReader(dataStream, nodeEntryCache, dataBlockEntryCache, cachedHeaderHolder),
                    CreatePropertyValueProcessor(dataStream, nodeEntryCache, dataBlockEntryCache, cachedHeaderHolder));
        }

        private static IPropertyValueReader CreatePropertyValueProcessor(
            Stream dataStream,
            ICache<NID[], NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new PropertyValueReader(
                    CreateHeapOnNodeReader(dataStream, nodeEntryCache, dataBlockEntryCache, cachedHeaderHolder),
                    CreateExternalDataBlockReader(dataStream, nodeEntryCache, dataBlockEntryCache, cachedHeaderHolder));
        }
    }
}

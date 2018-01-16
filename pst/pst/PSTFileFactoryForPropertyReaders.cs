using pst.encodables.ndb;
using pst.impl.decoders.ltp.hn;
using pst.impl.decoders.messaging;
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
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new TableContextBasedPropertyReader(
                    CreateRowMatrixReader(dataStream, nodeEntryCache, dataBlockEntryCache),
                    CreatePropertyValueProcessor(dataStream, nodeEntryCache, dataBlockEntryCache));
        }

        private static IPropertyNameToIdMap CreatePropertyIdToNameMap(
            Stream dataStream,
            ICache<NID[], NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new PropertyNameToIdMap(
                    new NAMEIDDecoder(),
                    CreatePropertyContextBasedPropertyReader(dataStream, nodeEntryCache, dataBlockEntryCache));
        }

        private static IPropertyContextBasedPropertyReader CreatePropertyContextBasedPropertyReader(
            Stream dataStream,
            ICache<NID[], NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new PropertyContextBasedPropertyReader(
                    CreatePropertyIdBasedBTreeOnHeapReader(dataStream, nodeEntryCache, dataBlockEntryCache),
                    CreatePropertyValueProcessor(dataStream, nodeEntryCache, dataBlockEntryCache));
        }

        private static IPropertyValueReader CreatePropertyValueProcessor(
            Stream dataStream,
            ICache<NID[], NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new PropertyValueReader(
                    new HNIDDecoder(),
                    CreateHeapOnNodeReader(dataStream, nodeEntryCache, dataBlockEntryCache),
                    CreateExternalDataBlockReader(dataStream, nodeEntryCache, dataBlockEntryCache));
        }
    }
}

using pst.encodables.ndb;
using pst.impl.ltp.tc;
using pst.impl.ndb;
using pst.interfaces;
using pst.interfaces.ltp.tc;
using pst.interfaces.ndb;
using System.IO;

namespace pst
{
    public partial class PSTFile
    {
        private static ITableContextReader CreateTableContextReader(
            Stream stream,
            ICache<NID[], NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new TableContextReader(
                    CreateRowIndexReader(stream, nodeEntryCache, dataBlockEntryCache, cachedHeaderHolder),
                    CreateRowMatrixReader(stream, nodeEntryCache, dataBlockEntryCache, cachedHeaderHolder));
        }

        private static IRowIndexReader CreateRowIndexReader(
            Stream dataStream,
            ICache<NID[], NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new RowIndexReader(
                    CreateHeapOnNodeReader(dataStream, nodeEntryCache, dataBlockEntryCache, cachedHeaderHolder),
                    CreateInt32BasedBTreeOnHeapReader(dataStream, nodeEntryCache, dataBlockEntryCache, cachedHeaderHolder));
        }

        private static IRowMatrixReader CreateRowMatrixReader(
            Stream dataStream,
            ICache<NID[], NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new RowMatrixReader(
                    CreateHeapOnNodeReader(dataStream, nodeEntryCache, dataBlockEntryCache, cachedHeaderHolder),
                    new RowValuesExtractor(),
                    CreateExternalDataBlockReader(dataStream, nodeEntryCache, dataBlockEntryCache, cachedHeaderHolder));
        }

        private static IDataTreeReader CreateExternalDataBlockReader(
            Stream dataStream,
            ICache<NID[], NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return new DataTreeReader(
                CreateNodeEntryFinder(dataStream, nodeEntryCache, dataBlockEntryCache, cachedHeaderHolder),
                CreateDataBlockEntryFinder(dataStream, dataBlockEntryCache, cachedHeaderHolder),
                CreateDataBlockReader(dataStream, dataBlockEntryCache, cachedHeaderHolder),
                CreateBlockEncoding(dataStream, cachedHeaderHolder));
        }
    }
}

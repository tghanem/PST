using pst.encodables.ndb;
using pst.impl.converters;
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
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new TableContextReader(
                    CreateRowIndexReader(stream, nodeEntryCache, dataBlockEntryCache),
                    CreateRowMatrixReader(stream, nodeEntryCache, dataBlockEntryCache));
        }

        private static IRowIndexReader CreateRowIndexReader(
            Stream dataStream,
            ICache<NID[], NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new RowIndexReader<NID>(
                    CreateHeapOnNodeReader(dataStream, nodeEntryCache, dataBlockEntryCache),
                    CreateNIDBasedBTreeOnHeapReader(dataStream, nodeEntryCache, dataBlockEntryCache),
                    new DataRecordToTCROWIDConverter());
        }

        private static IRowMatrixReader CreateRowMatrixReader(
            Stream dataStream,
            ICache<NID[], NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new RowMatrixReader(
                    CreateHeapOnNodeReader(dataStream, nodeEntryCache, dataBlockEntryCache),
                    new RowValuesExtractor(),
                    CreateExternalDataBlockReader(dataStream, nodeEntryCache, dataBlockEntryCache));
        }

        private static IDataTreeReader CreateExternalDataBlockReader(
            Stream dataStream,
            ICache<NID[], NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return new DataTreeReader(
                CreateNodeEntryFinder(dataStream, nodeEntryCache, dataBlockEntryCache),
                CreateDataBlockEntryFinder(dataStream, dataBlockEntryCache),
                CreateDataBlockReader(dataStream, dataBlockEntryCache),
                CreateBlockEncoding(dataStream));
        }
    }
}

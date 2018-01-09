using pst.encodables.ndb;
using pst.impl.converters;
using pst.impl.decoders.ltp.hn;
using pst.impl.decoders.ltp.tc;
using pst.impl.decoders.ndb;
using pst.impl.ltp.tc;
using pst.interfaces;
using pst.interfaces.ltp.tc;
using pst.interfaces.ndb;
using System.IO;

namespace pst
{
    public partial class PSTFile
    {
        private static ITableContextReader CreateNIDBasedTableContextReader(
            Stream stream,
            ICache<NID[], NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new TableContextReader<NID>(
                    CreateNIDBasedRowIndexReader(stream, nodeEntryCache, dataBlockEntryCache),
                    CreateNIDBasedRowMatrixReader(stream, nodeEntryCache, dataBlockEntryCache));
        }

        private static IRowIndexReader<NID> CreateNIDBasedRowIndexReader(
            Stream dataStream,
            ICache<NID[], NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new RowIndexReader<NID>(
                    new TCINFODecoder(
                        new HIDDecoder(),
                        new TCOLDESCDecoder()),
                    CreateHeapOnNodeReader(dataStream, nodeEntryCache, dataBlockEntryCache),
                    CreateNIDBasedBTreeOnHeapReader(dataStream, nodeEntryCache, dataBlockEntryCache),
                    new DataRecordToTCROWIDConverter());
        }

        private static IRowMatrixReader CreateNIDBasedRowMatrixReader(
            Stream dataStream,
            ICache<NID[], NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new RowMatrixReader(
                    CreateHeapOnNodeReader(dataStream, nodeEntryCache, dataBlockEntryCache),
                    new RowValuesExtractor(),
                    CreateNodeEntryFinder(dataStream, nodeEntryCache, dataBlockEntryCache),
                    CreateDataBlockEntryFinder(dataStream, dataBlockEntryCache),
                    new HNIDDecoder(
                        new HIDDecoder(),
                        new NIDDecoder()),
                    new TCINFODecoder(
                        new HIDDecoder(),
                        new TCOLDESCDecoder()),
                    CreateDataBlockReader(dataStream, dataBlockEntryCache));
        }

        private static IRowMatrixReader CreateTagBasedRowMatrixReader(
            Stream dataStream,
            ICache<NID[], NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new RowMatrixReader(
                    CreateHeapOnNodeReader(dataStream, nodeEntryCache, dataBlockEntryCache),
                    new RowValuesExtractor(),
                    CreateNodeEntryFinder(dataStream, nodeEntryCache, dataBlockEntryCache),
                    CreateDataBlockEntryFinder(dataStream, dataBlockEntryCache),
                    new HNIDDecoder(
                        new HIDDecoder(),
                        new NIDDecoder()),
                    new TCINFODecoder(
                        new HIDDecoder(),
                        new TCOLDESCDecoder()),
                    CreateDataBlockReader(dataStream, dataBlockEntryCache));
        }
    }
}

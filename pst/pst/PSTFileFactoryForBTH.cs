using pst.encodables.ndb;
using pst.impl;
using pst.impl.decoders.ltp.bth;
using pst.impl.decoders.ltp.hn;
using pst.impl.decoders.messaging;
using pst.impl.ltp.bth;
using pst.impl.ltp.hn;
using pst.impl.ndb;
using pst.interfaces;
using pst.interfaces.ltp.bth;
using pst.interfaces.ltp.hn;
using pst.interfaces.ndb;
using System.IO;

namespace pst
{
    public partial class PSTFile
    {
        private static IBTreeOnHeapReader<PropertyId> CreatePropertyIdBasedBTreeOnHeapReader(
            Stream dataStream,
            ICache<NID[], NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new BTreeOnHeapReader<PropertyId>(
                    new HIDDecoder(),
                    new PropertyIdDecoder(),
                    new BTHHEADERDecoder(
                        new HIDDecoder()),
                    CreateHeapOnNodeReader(dataStream, nodeEntryCache, dataBlockEntryCache));
        }

        private static IBTreeOnHeapReader<NID> CreateNIDBasedBTreeOnHeapReader(
            Stream dataStream,
            ICache<NID[], NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new BTreeOnHeapReader<NID>(
                    new HIDDecoder(),
                    new FuncBasedDecoder<NID>(NID.OfValue), 
                    new BTHHEADERDecoder(
                        new HIDDecoder()),
                    CreateHeapOnNodeReader(dataStream, nodeEntryCache, dataBlockEntryCache));
        }

        private static IHeapOnNodeReader CreateHeapOnNodeReader(
            Stream dataStream,
            ICache<NID[], NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new HeapOnNodeReader(
                    new HNHDRDecoder(
                        new HIDDecoder()),
                    new HNPAGEHDRDecoder(),
                    new HNPAGEMAPDecoder(),
                    new HNBITMAPHDRDecoder(),
                    new HeapOnNodeItemsLoader(),
                    new DataTreeReader(
                        CreateNodeEntryFinder(dataStream, nodeEntryCache, dataBlockEntryCache),
                        CreateDataBlockEntryFinder(dataStream, dataBlockEntryCache),
                        CreateDataBlockReader(dataStream, dataBlockEntryCache),
                        CreateBlockEncoding(dataStream)));
        }
    }
}

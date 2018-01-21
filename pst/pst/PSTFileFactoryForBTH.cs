using pst.encodables.ndb;
using pst.impl;
using pst.impl.decoders.messaging;
using pst.impl.ltp.bth;
using pst.impl.ltp.hn;
using pst.impl.ndb;
using pst.interfaces;
using pst.interfaces.ltp.bth;
using pst.interfaces.ltp.hn;
using pst.interfaces.ndb;
using System;
using System.IO;

namespace pst
{
    public partial class PSTFile
    {
        private static IBTreeOnHeapReader<PropertyId> CreatePropertyIdBasedBTreeOnHeapReader(
            Stream dataStream,
            ICache<NID[], NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new BTreeOnHeapReader<PropertyId>(
                    new PropertyIdDecoder(),
                    CreateHeapOnNodeReader(dataStream, nodeEntryCache, dataBlockEntryCache, cachedHeaderHolder));
        }

        private static IBTreeOnHeapReader<int> CreateInt32BasedBTreeOnHeapReader(
            Stream dataStream,
            ICache<NID[], NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new BTreeOnHeapReader<int>(
                    new FuncBasedDecoder<int>(d => BitConverter.ToInt32(d.Value, 0)),
                    CreateHeapOnNodeReader(dataStream, nodeEntryCache, dataBlockEntryCache, cachedHeaderHolder));
        }

        private static IHeapOnNodeReader CreateHeapOnNodeReader(
            Stream dataStream,
            ICache<NID[], NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new HeapOnNodeReader(
                    new HeapOnNodeItemsLoader(),
                    new DataTreeReader(
                        CreateNodeEntryFinder(dataStream, nodeEntryCache, dataBlockEntryCache, cachedHeaderHolder),
                        CreateDataBlockEntryFinder(dataStream, dataBlockEntryCache, cachedHeaderHolder),
                        CreateDataBlockReader(dataStream, dataBlockEntryCache, cachedHeaderHolder),
                        CreateBlockEncoding(dataStream, cachedHeaderHolder)));
        }
    }
}

using pst.encodables.ndb;
using pst.encodables.ndb.blocks.data;
using pst.encodables.ndb.btree;
using pst.impl;
using pst.impl.decoders.messaging;
using pst.impl.ltp.bth;
using pst.impl.ltp.hn;
using pst.interfaces;
using pst.interfaces.ltp.bth;
using pst.interfaces.ltp.hn;
using System;
using System.IO;

namespace pst
{
    public partial class PSTFile
    {
        private static IBTreeOnHeapReader<PropertyId> CreatePropertyIdBasedBTreeOnHeapReader(
            Stream dataStream,
            ICache<BID, BTPage> cachedBBTNodes,
            ICache<BID, InternalDataBlock> cachedInternalDataBlocks,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new BTreeOnHeapReader<PropertyId>(
                    new PropertyIdDecoder(),
                    CreateHeapOnNodeReader(dataStream, cachedBBTNodes, cachedInternalDataBlocks, cachedHeaderHolder));
        }

        private static IBTreeOnHeapReader<int> CreateInt32BasedBTreeOnHeapReader(
            Stream dataStream,
            ICache<BID, BTPage> cachedBBTNodes,
            ICache<BID, InternalDataBlock> cachedInternalDataBlocks,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new BTreeOnHeapReader<int>(
                    new FuncBasedDecoder<int>(d => BitConverter.ToInt32(d.Value, 0)),
                    CreateHeapOnNodeReader(dataStream, cachedBBTNodes, cachedInternalDataBlocks, cachedHeaderHolder));
        }

        private static IHeapOnNodeReader CreateHeapOnNodeReader(
            Stream dataStream,
            ICache<BID, BTPage> cachedBBTNodes,
            ICache<BID, InternalDataBlock> cachedInternalDataBlocks,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new HeapOnNodeReader(
                    new HeapOnNodeItemsLoader(),
                    CreateDataTreeReader(dataStream, cachedBBTNodes, cachedInternalDataBlocks, cachedHeaderHolder));
        }
    }
}

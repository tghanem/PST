using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces.btree;
using pst.interfaces.io;
using pst.impl.btree;
using pst.impl.decoders.ndb;
using pst.impl.decoders.primitives;
using pst.impl.decoders.ndb.btree;

namespace pst.impl.ndb.bbt
{
    class BlockBTree
    {
        private readonly IBTreeKeyFinder<LBBTEntry, BID> bbtreeKeyFinder;

        public BlockBTree(IDataReader dataReader, BREF rootNodeReference)
        {
            bbtreeKeyFinder =
                new BTreeKeyFinder<BTPage, BREF, IBBTEntry, LBBTEntry, BID>(
                    new BTreeNodeKeyLocator<BTPage, IBBTEntry, BID>(
                        new ComparerThatFindsTheFirstKeyThatIsLargerThanTheReferenceKey<IBBTEntry, BID>(
                            new BIDFromIBBTEntryExtractor()),
                        new IBBTEntriesFromBTPageExtractor(
                            new IBBTEntryDecoder(
                                new BIDDecoder(),
                                new BREFDecoder(
                                    new BIDDecoder(),
                                    new IBDecoder())))),
                    new BTreeNodeKeyLocator<BTPage, LBBTEntry, BID>(
                        new ComparerThatFindsTheFirstKeyThatMatchesTheReferenceKey<LBBTEntry, BID>(
                            new BIDFromLBBTEntryExtractor()),
                        new LBBTEntriesFromBTPageExtractor(
                            new LBBTEntryDecoder(
                                new BREFDecoder(
                                    new BIDDecoder(),
                                    new IBDecoder()),
                                new Int32Decoder()))),
                    new BTPageLoader(
                        dataReader,
                        new BTPageDecoder(
                            new Int32Decoder(),
                            new PageTrailerDecoder(
                                new Int32Decoder(),
                                new BIDDecoder()))),
                    rootNodeReference,
                    new BREFFromIBBTEntryExtractor(),
                    new PageLevelFromBTPageExtractor());
        }

        public LBBTEntry Find(BID blockId)
        {
            return bbtreeKeyFinder.Find(blockId).Value;
        }
    }
}

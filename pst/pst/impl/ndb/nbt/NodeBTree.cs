using pst.encodables.ndb.btree;
using pst.interfaces.btree;
using pst.impl.btree;
using pst.encodables.ndb;
using pst.impl.decoders.ndb;
using pst.interfaces.io;
using pst.impl.decoders.ndb.btree;
using pst.impl.decoders.primitives;

namespace pst.impl.ndb.nbt
{
    class NodeBTree
    {
        private readonly IBTreeKeyFinder<LNBTEntry, NID> nodeBTreeEntryFinder;

        public NodeBTree(IDataReader dataReader, BREF rootNodeReference)
        {
            nodeBTreeEntryFinder =
                new BTreeKeyFinder<BTPage, BREF, INBTEntry, LNBTEntry, NID>(
                    new BTreeNodeKeyLocator<BTPage, INBTEntry, NID>(
                        new ComparerThatFindsTheFirstKeyThatIsLargerThanTheReferenceKey<INBTEntry, NID>(
                            new NIDFromINBTEntryExtractor()),
                        new INBTEntriesFromBTPageExtractor(
                            new INBTEntryDecoder(
                                new NIDDecoder(),
                                new BREFDecoder(
                                    new BIDDecoder(),
                                    new IBDecoder())))),
                    new BTreeNodeKeyLocator<BTPage, LNBTEntry, NID>(
                        new ComparerThatFindsTheFirstKeyThatMatchesTheReferenceKey<LNBTEntry, NID>(
                            new NIDFromLNBTEntryExtractor()),
                        new LNBTEntriesFromBTPageExtractor(
                            new LNBTEntryDecoder(
                                new NIDDecoder(),
                                new BIDDecoder()))),
                    new BTPageLoader(
                        dataReader,
                        new BTPageDecoder(
                            new Int32Decoder(),
                            new PageTrailerDecoder(
                                new Int32Decoder(),
                                new BIDDecoder()))),
                    rootNodeReference,
                    new BREFFromINBTEntryExtractor(),
                    new PageLevelFromBTPageExtractor());
        }

        public LNBTEntry Find(NID nid)
        {
            return nodeBTreeEntryFinder.Find(nid).Value;
        }
    }
}

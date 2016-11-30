using pst.core;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.impl.btree;
using pst.impl.decoders.ltp.hn;
using pst.impl.decoders.ndb;
using pst.impl.decoders.ndb.btree;
using pst.impl.decoders.primitives;
using pst.impl.io;
using pst.impl.ndb;
using pst.impl.ndb.bbt;
using pst.impl.ndb.nbt;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.io;
using pst.interfaces.ndb;
using System.IO;

namespace pst
{
    public class PSTFile
    {
        private readonly NodeBTree nodeBTree;
        private readonly IOrderedDataBlockCollectionLoader nodeOrderedDataBlockCollectionLoader;

        public PSTFile(string filePath)
        {
            var fileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite);

            var dataReader = new StreamBasedDataReader(fileStream);

            var headerDecoder =
                new HeaderDecoder(
                    new Int32Decoder(),
                    new RootDecoder(
                        new Int32Decoder(),
                        new Int64Decoder(),
                        new BREFDecoder(
                             new BIDDecoder(),
                             new IBDecoder())),
                    new BIDDecoder(),
                    new NIDDecoder());

            var header =
                headerDecoder.Decode(dataReader.Read(IB.Zero, 564));

            nodeBTree =
                new NodeBTree(dataReader, header.Root.NBTRootPage);

            nodeOrderedDataBlockCollectionLoader =
                new OrderedNodeDataBlockCollectionLoader(
                    CreateBlockBTreeKeyFinder(
                        dataReader,
                        header.Root.BBTRootPage),
                    new OrderedDataBlockCollectionFactory(
                        dataReader));
        }

        public MessageStore GetMessageStore()
        {
            var lnbtEntry =
                nodeBTree.Find(new NID(0x21));

            var orderedDataBlockCollection =
                nodeOrderedDataBlockCollectionLoader.Load(lnbtEntry.DataBlockId);

            var hnHDRDecoder =
                new HNHDRDecoder(
                    new Int32Decoder());

            var hnHDR =
                hnHDRDecoder.Decode(orderedDataBlockCollection.Value.GetDataBlockAt(0).Data);

            return null;
        }

        private IBTreeKeyFinder<LBBTEntry, BID> CreateBlockBTreeKeyFinder(IDataReader dataReader, BREF rootNodeReference)
        {
            return
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
    }
}

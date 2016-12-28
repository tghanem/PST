using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.impl.io;
using System.Collections.Generic;
using System.IO;

namespace pst
{
    public partial class PSTFile
    {
        public static PSTFile Open(Stream stream)
        {
            var streamReader =
                new StreamBasedBlockReader(stream);

            var nodeBTree = new Dictionary<NID, LNBTEntry>();

            var blockBTree = new Dictionary<BID, LBBTEntry>();

            var header =
                PSTServiceFactory
                .CreateHeaderDecoder()
                .Decode(streamReader.Read(BREF.OfValue(BID.OfValue(0), IB.Zero), 546));

            foreach (var entry in PSTServiceFactory
                                    .CreateNodeBTreeLeafKeysEnumerator()
                                    .Enumerate(streamReader, header.Root.NBTRootPage))
            {
                nodeBTree.Add(entry.NodeId, entry);
            }

            foreach (var entry in PSTServiceFactory
                                    .CreateBlockBTreeLeafKeysEnumerator()
                                    .Enumerate(streamReader, header.Root.BBTRootPage))
            {
                blockBTree.Add(entry.BlockReference.BlockId, entry);
            }

            var subFoldersFactory =
                new FolderSubFoldersFactory(
                    PSTServiceFactory.CreateSubnodeBTreeLeafKeysEnumerator(),
                    PSTServiceFactory.CreatePropertiesFromTableContextRowLoader(),
                    PSTServiceFactory.CreateRowMatrixLoader(),
                    streamReader,
                    blockBTree,
                    nodeBTree);

            return
                new PSTFile(
                    new MessageStoreFactory(
                        PSTServiceFactory.CreatePropertiesFromPropertyContextLoader(),
                        subFoldersFactory,
                        streamReader,
                        blockBTree,
                        nodeBTree),
                    new FolderFactory(
                        PSTServiceFactory.CreatePropertiesFromPropertyContextLoader(),
                        subFoldersFactory,
                        streamReader,
                        blockBTree,
                        nodeBTree));
        }
    }
}

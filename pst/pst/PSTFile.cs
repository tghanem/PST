using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.impl.io;
using pst.interfaces.io;
using pst.utilities;
using System.Collections.Generic;
using System.IO;

namespace pst
{
    public class PSTFile
    {
        private readonly Dictionary<NID, LNBTEntry> nodeBTree;
        private readonly Dictionary<BID, LBBTEntry> blockBTree;

        private readonly IDataBlockReader<BREF> streamReader;

        public PSTFile(Stream stream)
        {
            streamReader =
                new StreamBasedBlockReader(stream);

            var header =
                PSTServices.HeaderDecoder.Decode(
                    streamReader.Read(BREF.OfValue(BID.OfValue(0), IB.Zero), 546));

            nodeBTree = new Dictionary<NID, LNBTEntry>();
            blockBTree = new Dictionary<BID, LBBTEntry>();

            foreach (var entry in PSTServices.NodeBTreeKeysEnumerator.Enumerate(streamReader, header.Root.NBTRootPage))
            {
                nodeBTree.Add(entry.NodeId, entry);
            }

            foreach (var entry in PSTServices.BlockBTreeKeysEnumerator.Enumerate(streamReader, header.Root.BBTRootPage))
            {
                blockBTree.Add(entry.BlockReference.BlockId, entry);
            }
        }

        public MessageStore GetMessageStore()
        {
            var nbtEntry = nodeBTree[Globals.NID_MESSAGE_STORE];

            var bbtEntry = blockBTree[nbtEntry.DataBlockId];

            var properties = PSTServices.PropertiesFromPropertyContextLoader.Load(
                new LBBTEntryBlockReaderAdapter(streamReader),
                blockBTree,
                bbtEntry);

            return new MessageStore(properties);
        }

        public Folder GetRootFolder()
        {
            var nbtEntry = nodeBTree[Globals.NID_ROOT_FOLDER];

            var bbtEntry = blockBTree[nbtEntry.DataBlockId];

            var properties = PSTServices.PropertiesFromPropertyContextLoader.Load(
                new LBBTEntryBlockReaderAdapter(streamReader),
                blockBTree,
                bbtEntry);

            return new Folder(properties);
        }
    }
}

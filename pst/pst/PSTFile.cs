using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.impl.io;
using pst.utilities;
using System.Collections.Generic;
using System.IO;

namespace pst
{
    public class PSTFile
    {
        private readonly Dictionary<NID, LNBTEntry> nodeBTree;
        private readonly Dictionary<BID, LBBTEntry> blockBTree;

        public PSTFile(Stream stream)
        {
            var reader =
                new StreamBasedBlockReader(stream);

            var header =
                PSTServices.HeaderDecoder.Decode(
                    reader.Read(BREF.OfValue(BID.OfValue(0), IB.Zero), 546));

            nodeBTree = new Dictionary<NID, LNBTEntry>();

            foreach (var entry in PSTServices.NodeBTreeKeysEnumerator.Enumerate(reader, header.Root.NBTRootPage))
            {
                nodeBTree.Add(entry.NodeId, entry);
            }

            foreach (var entry in PSTServices.BlockBTreeKeysEnumerator.Enumerate(reader, header.Root.BBTRootPage))
            {
                blockBTree.Add(entry.BlockReference.BlockId, entry);
            }
        }

        public MessageStore GetMessageStore()
        {
            return null;
        }

        public Folder GetRootFolder()
        {
            return null;
        }
    }
}

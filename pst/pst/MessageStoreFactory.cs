using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.impl.io;
using pst.interfaces;
using pst.interfaces.io;
using pst.interfaces.ltp.pc;
using pst.utilities;
using System.Collections.Generic;

namespace pst
{
    class MessageStoreFactory : IFactory<NID, MessageStore>
    {
        private readonly IPropertiesFromPropertyContextLoader propertiesFromPropertyContextLoader;
        private readonly IFactory<NID, Folder[]> subFoldersFactory;

        private readonly IDataBlockReader<BREF> streamReader;
        private readonly Dictionary<BID, LBBTEntry> blockBTree;
        private readonly Dictionary<NID, LNBTEntry> nodeBTree;

        public MessageStoreFactory(
            IPropertiesFromPropertyContextLoader propertiesFromPropertyContextLoader,
            IFactory<NID, Folder[]> subFoldersFactory,
            IDataBlockReader<BREF> streamReader,
            Dictionary<BID, LBBTEntry> blockBTree,
            Dictionary<NID, LNBTEntry> nodeBTree)
        {
            this.propertiesFromPropertyContextLoader = propertiesFromPropertyContextLoader;
            this.subFoldersFactory = subFoldersFactory;
            this.streamReader = streamReader;
            this.blockBTree = blockBTree;
            this.nodeBTree = nodeBTree;
        }

        public MessageStore Create(NID nodeId)
        {
            var nbtEntry = nodeBTree[Globals.NID_MESSAGE_STORE];

            var bbtEntry = blockBTree[nbtEntry.DataBlockId];

            var properties =
                propertiesFromPropertyContextLoader.Load(
                    new LBBTEntryBlockReaderAdapter(streamReader),
                    new DictionaryBasedMapper<BID, LBBTEntry>(blockBTree),
                    bbtEntry);

            return new MessageStore(properties);
        }
    }
}

using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.impl.io;
using pst.interfaces;
using pst.interfaces.io;
using pst.interfaces.ltp.pc;
using pst.interfaces.ltp.tc;
using pst.utilities;
using System.Collections.Generic;

namespace pst
{
    public partial class PSTFile
    {
        private readonly Dictionary<NID, LNBTEntry> nodeBTree;
        private readonly Dictionary<BID, LBBTEntry> blockBTree;
        private readonly IDataBlockReader<BREF> streamReader;

        private readonly IPropertiesFromTableContextRowLoader propertiesFromTableContextRowLoader;
        private readonly IPropertiesFromPropertyContextLoader propertiesFromPropertyContextLoader;
        private readonly IRowMatrixLoader rowMatrixLoader;
        private readonly IDecoder<NID> nidDecoder;

        private PSTFile(
            IDataBlockReader<BREF> streamReader,
            Dictionary<NID, LNBTEntry> nodeBTree,
            Dictionary<BID, LBBTEntry> blockBTree,
            IPropertiesFromTableContextRowLoader propertiesFromTableContextRowLoader,
            IPropertiesFromPropertyContextLoader propertiesFromPropertyContextLoader,
            IRowMatrixLoader rowMatrixLoader,
            IDecoder<NID> nidDecoder)
        {
            this.nodeBTree = nodeBTree;
            this.blockBTree = blockBTree;
            this.streamReader = streamReader;
            this.propertiesFromPropertyContextLoader = propertiesFromPropertyContextLoader;
            this.propertiesFromTableContextRowLoader = propertiesFromTableContextRowLoader;
            this.rowMatrixLoader = rowMatrixLoader;
            this.nidDecoder = nidDecoder;
        }

        public MessageStore GetMessageStore()
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

        public Folder GetRootFolder()
        {
            var lnbtEntry = nodeBTree[new NID(0x12d)];

            return
                new Folder(
                    new Dictionary<PropertyId, PropertyValue>(),
                    propertiesFromTableContextRowLoader,
                    streamReader,
                    rowMatrixLoader,
                    nidDecoder,
                    nodeBTree,
                    blockBTree,
                    lnbtEntry);
        }
    }
}

using pst.encodables.ndb;
using pst.encodables.ndb.blocks.subnode;
using pst.encodables.ndb.btree;
using pst.impl.io;
using pst.impl.ndb.subnodebtree;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.io;
using pst.interfaces.ltp.hn;
using pst.interfaces.ltp.tc;
using pst.utilities;
using System.Collections.Generic;
using System.Linq;

namespace pst
{
    class FolderSubFoldersFactory : IFactory<NID, Folder[]>
    {
        private readonly IBTreeLeafKeysEnumeratorThatDoesntKnowHowToMapTheKeyToNodeReference<SLEntry, SIEntry, LBBTEntry> subNodesBTreeLeafKeysEnumerator;
        private readonly IPropertiesFromTableContextRowLoader propertiesFromTableContextRowLoader;
        private readonly IHeapOnNodeLoader heapOnNodeLoader;
        private readonly IRowMatrixLoader rowMatrixLoader;

        private readonly IDataBlockReader<BREF> streamReader;
        private readonly Dictionary<BID, LBBTEntry> blockBTree;
        private readonly Dictionary<NID, LNBTEntry> nodeBTree;

        public FolderSubFoldersFactory(
            IBTreeLeafKeysEnumeratorThatDoesntKnowHowToMapTheKeyToNodeReference<SLEntry, SIEntry, LBBTEntry> subNodesBTreeLeafKeysEnumerator,
            IPropertiesFromTableContextRowLoader propertiesFromTableContextRowLoader,
            IRowMatrixLoader rowMatrixLoader,
            IHeapOnNodeLoader heapOnNodeLoader,
            IDataBlockReader<BREF> streamReader,
            Dictionary<BID, LBBTEntry> blockBTree,
            Dictionary<NID, LNBTEntry> nodeBTree)
        {
            this.subNodesBTreeLeafKeysEnumerator = subNodesBTreeLeafKeysEnumerator;
            this.propertiesFromTableContextRowLoader = propertiesFromTableContextRowLoader;
            this.heapOnNodeLoader = heapOnNodeLoader;
            this.rowMatrixLoader = rowMatrixLoader;
            this.streamReader = streamReader;
            this.blockBTree = blockBTree;
            this.nodeBTree = nodeBTree;
        }

        public Folder[] Create(NID nodeId)
        {
            var lnbtEntryForHierarchyTable =
                nodeBTree[nodeId.ChangeType(Globals.NID_TYPE_HIERARCHY_TABLE)];

            var bbtEntry = blockBTree[lnbtEntryForHierarchyTable.DataBlockId];

            var rowMatrix =
                rowMatrixLoader
                .Load(
                    new LBBTEntryBlockReaderAdapter(streamReader),
                    GetMapperForSubnodes(lnbtEntryForHierarchyTable.SubnodeBlockId),
                    new DictionaryBasedMapper<BID, LBBTEntry>(blockBTree),
                    bbtEntry);

            var folders = new List<Folder>();

            foreach (var row in rowMatrix)
            {
                var heapOnNode =
                    heapOnNodeLoader
                    .Load(
                        new LBBTEntryBlockReaderAdapter(streamReader),
                        new DictionaryBasedMapper<BID, LBBTEntry>(blockBTree),
                        blockBTree[lnbtEntryForHierarchyTable.DataBlockId]);

                var properties =
                    propertiesFromTableContextRowLoader
                    .Load(
                        heapOnNode,
                        row);

                folders.Add(
                    new Folder(
                        properties,
                        this,
                        row.RowId));
            }

            return folders.ToArray();
        }

        public IMapper<NID, SLEntry> GetMapperForSubnodes(BID subnodeBlockId)
        {
            if (subnodeBlockId.Value == 0)
            {
                return
                    new DictionaryBasedMapper<NID, SLEntry>(
                        new Dictionary<NID, SLEntry>());
            }
            else
            {
                var bbtEntryForSubnode = blockBTree[subnodeBlockId];

                return
                    new DictionaryBasedMapper<NID, SLEntry>(
                        subNodesBTreeLeafKeysEnumerator
                        .Enumerate(
                            new LBBTEntryBlockReaderAdapter(streamReader),
                            new SIEntryToLBBTEntryMapper(blockBTree),
                            bbtEntryForSubnode)
                        .ToDictionary(
                            k => k.LocalSubnodeId,
                            k => k));
            }
        }
    }
}

using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.impl.io;
using pst.interfaces;
using pst.interfaces.io;
using pst.utilities;
using System.Collections.Generic;
using System.Text;

namespace pst
{
    public class Folder
    {
        private readonly IDictionary<PropertyId, PropertyValue> properties;
        private readonly IDataBlockReader<BREF> streamReader;
        private readonly IDecoder<NID> nidDecoder;

        private readonly Dictionary<NID, LNBTEntry> nodeBTree;
        private readonly Dictionary<BID, LBBTEntry> blockBTree;
        private readonly LNBTEntry lnbtEntry;

        internal Folder(
            IDictionary<PropertyId, PropertyValue> properties,
            IDataBlockReader<BREF> streamReader,
            IDecoder<NID> nidDecoder,
            Dictionary<NID, LNBTEntry> nodeBTree,
            Dictionary<BID, LBBTEntry> blockBTree,
            LNBTEntry lnbtEntry)
        {
            this.lnbtEntry = lnbtEntry;
            this.nodeBTree = nodeBTree;
            this.nidDecoder = nidDecoder;
            this.properties = properties;
            this.blockBTree = blockBTree;
            this.streamReader = streamReader;
        }

        public Folder[] GetSubFolders()
        {
            var lnbtEntryForHierarchyTable =
                nodeBTree[lnbtEntry.NodeId.ChangeType(Globals.NID_TYPE_HIERARCHY_TABLE)];

            var table =
                PSTServices
                .RowMatrixLoader
                .Load(
                    new LBBTEntryBlockReaderAdapter(streamReader),
                    PSTServices.GetMapperForSubnodes(
                        blockBTree,
                        streamReader,
                        lnbtEntryForHierarchyTable.SubnodeBlockId),
                    new DictionaryBasedMapper<BID, LBBTEntry>(blockBTree),
                    blockBTree[lnbtEntryForHierarchyTable.DataBlockId]);

            var folders = new List<Folder>();

            foreach (var row in table)
            {
                var properties =
                    PSTServices.PropertiesFromTableContextRowLoader.Load(
                        new LBBTEntryBlockReaderAdapter(streamReader),
                        PSTServices.GetMapperForSubnodes(
                            blockBTree,
                            streamReader,
                            lnbtEntryForHierarchyTable.SubnodeBlockId),
                        new DictionaryBasedMapper<BID, LBBTEntry>(blockBTree),
                        blockBTree[lnbtEntryForHierarchyTable.DataBlockId],
                        row);

                folders.Add(
                    new Folder(
                        properties,
                        streamReader,
                        nidDecoder,
                        nodeBTree,
                        blockBTree,
                        nodeBTree[row.RowId]));
            }

            return folders.ToArray();
        }

        public string DisplayName
        {
            get
            {
                var propertyId = new PropertyId(0x3001);

                if (!properties.ContainsKey(propertyId))
                    return null;

                return Encoding.Unicode.GetString(properties[propertyId].Value);
            }
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}

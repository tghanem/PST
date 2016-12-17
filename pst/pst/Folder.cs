using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.impl.io;
using pst.interfaces.io;
using pst.utilities;
using System.Collections.Generic;
using System.Text;

namespace pst
{
    public class Folder
    {
        private readonly IDictionary<PropertyId, PropertyValue> properties;
        private readonly Dictionary<BID, LBBTEntry> blockBTree;
        private readonly IDataBlockReader<BREF> streamReader;
        private readonly LNBTEntry lnbtEntry;

        internal Folder(
            IDictionary<PropertyId, PropertyValue> properties,
            Dictionary<BID, LBBTEntry> blockBTree,
            IDataBlockReader<BREF> streamReader,
            LNBTEntry lnbtEntry)
        {
            this.properties = properties;
            this.blockBTree = blockBTree;
            this.streamReader = streamReader;
            this.lnbtEntry = lnbtEntry;
        }

        public Folder[] GetSubFolders()
        {
            var table =
                PSTServices
                .RowMatrixLoader
                .Load(
                    new LBBTEntryBlockReaderAdapter(streamReader),
                    PSTServices.GetMapperForSubnodes(
                        blockBTree,
                        streamReader,
                        lnbtEntry.SubnodeBlockId),
                    new DictionaryBasedMapper<BID, LBBTEntry>(blockBTree),
                    blockBTree[lnbtEntry.DataBlockId]);

            var folders = new List<Folder>();

            foreach (var row in table)
            {
                var properties =
                    PSTServices.PropertiesFromTableContextRowLoader.Load(
                        new LBBTEntryBlockReaderAdapter(streamReader),
                        PSTServices.GetMapperForSubnodes(
                            blockBTree,
                            streamReader,
                            lnbtEntry.SubnodeBlockId),
                        new DictionaryBasedMapper<BID, LBBTEntry>(blockBTree),
                        blockBTree[lnbtEntry.DataBlockId],
                        row);

                folders.Add(
                    new Folder(
                        properties,
                        blockBTree,
                        streamReader,
                        null));
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
    }
}

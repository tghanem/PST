using pst.encodables;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ltp;
using pst.interfaces.ltp.pc;
using pst.interfaces.ltp.tc;
using pst.interfaces.messaging;
using pst.interfaces.messaging.model;
using pst.interfaces.messaging.model.changetracking;
using pst.interfaces.ndb;
using pst.utilities;
using System.Linq;

namespace pst
{
    public class Folder : ObjectBase
    {
        private readonly NodePath nodePath;
        private readonly IDecoder<NID> nidDecoder;
        private readonly IChangesTracker changesTracker;
        private readonly IEncoder<string> stringEncoder;
        private readonly INodeEntryFinder nodeEntryFinder;
        private readonly IRowIndexReader<NID> rowIndexReader;
        private readonly ITableContextReader tableContextReader;
        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader;
        private readonly ITableContextBasedPropertyReader<Tag> tableContextBasedPropertyReader;
        private readonly IUnallocatedNodeIdGenerator nodeIdGenerator;

        private bool subfoldersLoaded;
        private bool messagesLoaded;

        internal Folder(
            NodePath nodePath,
            IChangesTracker changesTracker,
            IEncoder<string> stringEncoder,
            IPropertyNameToIdMap propertyNameToIdMap,
            IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader,
            IDecoder<NID> nidDecoder,
            INodeEntryFinder nodeEntryFinder,
            IRowIndexReader<NID> rowIndexReader,
            ITableContextReader tableContextReader,
            ITableContextBasedPropertyReader<Tag> tableContextBasedPropertyReader,
            IUnallocatedNodeIdGenerator nodeIdGenerator) : base(nodePath, ObjectTypes.Folder, changesTracker, propertyNameToIdMap, propertyContextBasedPropertyReader)
        {
            this.nodePath = nodePath;
            this.nidDecoder = nidDecoder;
            this.changesTracker = changesTracker;
            this.stringEncoder = stringEncoder;
            this.nodeEntryFinder = nodeEntryFinder;
            this.rowIndexReader = rowIndexReader;
            this.tableContextReader = tableContextReader;
            this.tableContextBasedPropertyReader = tableContextBasedPropertyReader;
            this.nodeIdGenerator = nodeIdGenerator;
            this.propertyNameToIdMap = propertyNameToIdMap;
            this.propertyContextBasedPropertyReader = propertyContextBasedPropertyReader;
        }

        public Folder NewFolder(string displayName)
        {
            var childFolderNodePath = NodePath.OfValue(nodeIdGenerator.New());

            changesTracker.TrackNode(
                childFolderNodePath,
                ObjectTypes.Folder,
                ObjectStates.New,
                nodePath);

            changesTracker.SetProperty(
                childFolderNodePath,
                MAPIProperties.PidTagDisplayName,
                new PropertyValue(stringEncoder.Encode(displayName)));

            changesTracker.SetProperty(
                childFolderNodePath,
                MAPIProperties.PidTagContentCount,
                new PropertyValue(BinaryData.From(0)));

            changesTracker.SetProperty(
                childFolderNodePath,
                MAPIProperties.PidTagContentUnreadCount,
                new PropertyValue(BinaryData.From(0)));

            changesTracker.SetProperty(
                childFolderNodePath,
                MAPIProperties.PidTagSubfolders,
                new PropertyValue(BinaryData.From(false)));

            return
                new Folder(
                    childFolderNodePath,
                    changesTracker,
                    stringEncoder,
                    propertyNameToIdMap,
                    propertyContextBasedPropertyReader,
                    nidDecoder,
                    nodeEntryFinder,
                    rowIndexReader,
                    tableContextReader,
                    tableContextBasedPropertyReader,
                    nodeIdGenerator);
        }

        public Folder[] GetSubFolders()
        {
            if (!subfoldersLoaded)
            {
                TrackPreExistingChildren(Constants.NID_TYPE_HIERARCHY_TABLE, ObjectTypes.Folder);
                subfoldersLoaded = true;
            }

            return
                changesTracker
                .GetChildren(
                    parentNodePath: nodePath,
                    childType: ObjectTypes.Folder,
                    childStatePredicate: s => s == ObjectStates.New || s == ObjectStates.Loaded)
                .Select(
                    childNodePath =>
                    new Folder(
                        childNodePath,
                        changesTracker,
                        stringEncoder,
                        propertyNameToIdMap,
                        propertyContextBasedPropertyReader,
                        nidDecoder,
                        nodeEntryFinder,
                        rowIndexReader,
                        tableContextReader,
                        tableContextBasedPropertyReader,
                        nodeIdGenerator))
                .ToArray();
        }

        public Message[] GetMessages()
        {
            if (!messagesLoaded)
            {
                TrackPreExistingChildren(Constants.NID_TYPE_CONTENTS_TABLE, ObjectTypes.Message);
                messagesLoaded = true;
            }

            return
                changesTracker
                .GetChildren(
                    parentNodePath: nodePath,
                    childType: ObjectTypes.Message,
                    childStatePredicate: s => s == ObjectStates.New || s == ObjectStates.Loaded)
                .Select(
                    childNodePath =>
                        new Message(
                            childNodePath,
                            nidDecoder,
                            changesTracker,
                            nodeEntryFinder,
                            rowIndexReader,
                            tableContextReader,
                            propertyNameToIdMap,
                            propertyContextBasedPropertyReader,
                            tableContextBasedPropertyReader))
                .ToArray();
        }

        private void TrackPreExistingChildren(int tableContextNodeType, ObjectTypes childObjectType)
        {
            var childrenTableContextNodePath = new[] { nodePath.AllocatedId.ChangeType(tableContextNodeType) };

            foreach (var rowId in rowIndexReader.GetAllRowIds(childrenTableContextNodePath))
            {
                var childNodePath = NodePath.OfValue(AllocatedNodeId.OfValue(nidDecoder.Decode(rowId.RowId)));

                changesTracker.TrackNode(childNodePath, childObjectType, ObjectStates.Loaded, nodePath);
            }
        }
    }
}

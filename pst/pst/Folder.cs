using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ltp;
using pst.interfaces.ltp.tc;
using pst.interfaces.messaging;
using pst.interfaces.messaging.changetracking;
using pst.interfaces.model;
using pst.interfaces.ndb;
using pst.utilities;
using System.Linq;

namespace pst
{
    public class Folder : ObjectBase
    {
        private readonly NodePath nodePath;
        private readonly IChangesTracker changesTracker;
        private readonly IEncoder<string> stringEncoder;
        private readonly INodeEntryFinder nodeEntryFinder;
        private readonly IRowIndexReader rowIndexReader;
        private readonly ITableContextReader tableContextReader;
        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader;
        private readonly ITableContextBasedPropertyReader tableContextBasedPropertyReader;
        private readonly IUnallocatedNodeIdGenerator nodeIdGenerator;

        private bool subfoldersLoaded;
        private bool messagesLoaded;

        internal Folder(
            NodePath nodePath,
            IChangesTracker changesTracker,
            IEncoder<string> stringEncoder,
            IPropertyNameToIdMap propertyNameToIdMap,
            IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader,
            INodeEntryFinder nodeEntryFinder,
            IRowIndexReader rowIndexReader,
            ITableContextReader tableContextReader,
            ITableContextBasedPropertyReader tableContextBasedPropertyReader,
            IUnallocatedNodeIdGenerator nodeIdGenerator) : base(nodePath, changesTracker, propertyNameToIdMap, propertyContextBasedPropertyReader)
        {
            this.nodePath = nodePath;
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
                var childNodePath = NodePath.OfValue(AllocatedNodeId.OfValue(NID.OfValue(rowId.RowId)));

                changesTracker.TrackNode(childNodePath, childObjectType, ObjectStates.Loaded, nodePath);
            }
        }
    }
}

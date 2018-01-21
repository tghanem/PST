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
        private readonly ObjectPath objectPath;
        private readonly IObjectTracker objectTracker;
        private readonly IRecipientTracker recipientTracker;
        private readonly IEncoder<string> stringEncoder;
        private readonly INodeEntryFinder nodeEntryFinder;
        private readonly IRowIndexReader rowIndexReader;
        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader;
        private readonly ITableContextBasedPropertyReader tableContextBasedPropertyReader;
        private readonly INIDAllocator nodeIdAllocator;

        private bool subfoldersLoaded;
        private bool messagesLoaded;

        internal Folder(
            ObjectPath objectPath,
            IObjectTracker objectTracker,
            IRecipientTracker recipientTracker,
            IEncoder<string> stringEncoder,
            IPropertyNameToIdMap propertyNameToIdMap,
            IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader,
            INodeEntryFinder nodeEntryFinder,
            IRowIndexReader rowIndexReader,
            ITableContextBasedPropertyReader tableContextBasedPropertyReader,
            INIDAllocator nodeIdAllocator) : base(objectPath, objectTracker, propertyNameToIdMap, propertyContextBasedPropertyReader)
        {
            this.objectPath = objectPath;
            this.objectTracker = objectTracker;
            this.recipientTracker = recipientTracker;
            this.stringEncoder = stringEncoder;
            this.nodeEntryFinder = nodeEntryFinder;
            this.rowIndexReader = rowIndexReader;
            this.tableContextBasedPropertyReader = tableContextBasedPropertyReader;
            this.nodeIdAllocator = nodeIdAllocator;
            this.propertyNameToIdMap = propertyNameToIdMap;
            this.propertyContextBasedPropertyReader = propertyContextBasedPropertyReader;
        }

        public Folder NewFolder(string displayName)
        {
            var childFolderNodePath = objectPath.Add(nodeIdAllocator.Allocate(Constants.NID_TYPE_NORMAL_FOLDER));

            objectTracker.TrackObject(
                childFolderNodePath,
                ObjectTypes.Folder,
                ObjectStates.New);

            objectTracker.SetProperty(
                childFolderNodePath,
                MAPIProperties.PidTagDisplayName,
                new PropertyValue(stringEncoder.Encode(displayName)));

            objectTracker.SetProperty(
                childFolderNodePath,
                MAPIProperties.PidTagContentCount,
                new PropertyValue(BinaryData.OfValue(0)));

            objectTracker.SetProperty(
                childFolderNodePath,
                MAPIProperties.PidTagContentUnreadCount,
                new PropertyValue(BinaryData.OfValue(0)));

            objectTracker.SetProperty(
                childFolderNodePath,
                MAPIProperties.PidTagSubfolders,
                new PropertyValue(BinaryData.OfValue(false)));

            return
                new Folder(
                    childFolderNodePath,
                    objectTracker,
                    recipientTracker,
                    stringEncoder,
                    propertyNameToIdMap,
                    propertyContextBasedPropertyReader,
                    nodeEntryFinder,
                    rowIndexReader,
                    tableContextBasedPropertyReader,
                    nodeIdAllocator);
        }

        public Folder[] GetSubFolders()
        {
            if (!subfoldersLoaded)
            {
                TrackPreExistingChildren(Constants.NID_TYPE_HIERARCHY_TABLE, ObjectTypes.Folder);
                subfoldersLoaded = true;
            }

            return
                objectTracker
                .GetChildObjects(
                    objectPath: objectPath,
                    childType: ObjectTypes.Folder,
                    childStatePredicate: s => s == ObjectStates.New || s == ObjectStates.Loaded)
                .Select(
                    childNodePath =>
                    new Folder(
                        childNodePath,
                        objectTracker,
                        recipientTracker,
                        stringEncoder,
                        propertyNameToIdMap,
                        propertyContextBasedPropertyReader,
                        nodeEntryFinder,
                        rowIndexReader,
                        tableContextBasedPropertyReader,
                        nodeIdAllocator))
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
                objectTracker
                .GetChildObjects(
                    objectPath: objectPath,
                    childType: ObjectTypes.Message,
                    childStatePredicate: s => s == ObjectStates.New || s == ObjectStates.Loaded)
                .Select(
                    childNodePath =>
                        new Message(
                            childNodePath,
                            objectTracker,
                            recipientTracker,
                            nodeEntryFinder,
                            rowIndexReader,
                            propertyNameToIdMap,
                            propertyContextBasedPropertyReader,
                            tableContextBasedPropertyReader))
                .ToArray();
        }

        private void TrackPreExistingChildren(int tableContextNodeType, ObjectTypes childObjectType)
        {
            var childrenTableContextNodePath = new[] { objectPath.LocalNodeId.ChangeType(tableContextNodeType) };

            foreach (var rowId in rowIndexReader.GetAllRowIds(childrenTableContextNodePath))
            {
                var childNodePath = objectPath.Add(NID.OfValue(rowId.RowId));

                objectTracker.TrackObject(childNodePath, childObjectType, ObjectStates.Loaded);
            }
        }
    }
}

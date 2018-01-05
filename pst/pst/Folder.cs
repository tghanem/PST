using pst.core;
using pst.encodables;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ltp;
using pst.interfaces.ltp.pc;
using pst.interfaces.ltp.tc;
using pst.interfaces.messaging.model;
using pst.interfaces.messaging.model.changetracking;
using pst.interfaces.ndb;
using pst.utilities;
using System;
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
            ITableContextBasedPropertyReader<Tag> tableContextBasedPropertyReader) : base(nodePath, ObjectTypes.Folder, changesTracker, propertyNameToIdMap, propertyContextBasedPropertyReader)
        {
            this.nodePath = nodePath;
            this.nidDecoder = nidDecoder;
            this.changesTracker = changesTracker;
            this.stringEncoder = stringEncoder;
            this.nodeEntryFinder = nodeEntryFinder;
            this.rowIndexReader = rowIndexReader;
            this.tableContextReader = tableContextReader;
            this.tableContextBasedPropertyReader = tableContextBasedPropertyReader;
            this.propertyNameToIdMap = propertyNameToIdMap;
            this.propertyContextBasedPropertyReader = propertyContextBasedPropertyReader;
        }

        public Folder NewFolder(string displayName)
        {
            throw new NotImplementedException();
        }

        public Folder[] GetSubFolders()
        {
            var nodeIds = GetNodeIdsForSubFolders();

            if (nodeIds.HasNoValue)
            {
                return new Folder[0];
            }

            return
                nodeIds
                .Value
                .Select(
                    folderNodeId =>
                    new Folder(
                        NodePath.OfValue(folderNodeId), 
                        changesTracker,
                        stringEncoder,
                        propertyNameToIdMap,
                        propertyContextBasedPropertyReader,
                        nidDecoder,
                        nodeEntryFinder,
                        rowIndexReader,
                        tableContextReader,
                        tableContextBasedPropertyReader))
                .ToArray();
        }

        public Message[] GetMessages()
        {
            var nodeIds = GetNodeIdsForContents();

            if (nodeIds.HasNoValue)
            {
                return new Message[0];
            }

            return
                nodeIds
                .Value
                .Select(
                    messageNodeId =>
                    new Message(
                        NodePath.OfValue(messageNodeId),
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

        private Maybe<NodeId[]> GetNodeIdsForSubFolders()
        {
            if (nodePath.Id is AllocatedNodeId allocatedNodeId)
            {
                var hierarchyTableNodeId = NodePath.OfValue(allocatedNodeId.ChangeType(Constants.NID_TYPE_HIERARCHY_TABLE));

                return
                    rowIndexReader
                    .GetAllRowIds(hierarchyTableNodeId.AllocatedIds)
                    .Select(rowId => AllocatedNodeId.OfValue(nidDecoder.Decode(rowId.RowId)))
                    .ToArray();
            }

            throw new Exception("Unallocated node ids are not supported");
        }

        private Maybe<NodeId[]> GetNodeIdsForContents()
        {
            if (nodePath.Id is AllocatedNodeId allocatedNodeId)
            {
                var contentsTableNodeId = NodePath.OfValue(allocatedNodeId.ChangeType(Constants.NID_TYPE_CONTENTS_TABLE));

                return
                    rowIndexReader
                    .GetAllRowIds(contentsTableNodeId.AllocatedIds)
                    .Select(rowId => AllocatedNodeId.OfValue(nidDecoder.Decode(rowId.RowId)))
                    .ToArray();
            }

            throw new Exception("Unallocated node ids are not supported");
        }
    }
}

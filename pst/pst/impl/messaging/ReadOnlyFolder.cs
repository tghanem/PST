using pst.core;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ltp.tc;
using pst.interfaces.messaging;
using pst.utilities;
using System.Linq;

namespace pst.impl.messaging
{
    class ReadOnlyFolder : IReadOnlyFolder
    {
        private readonly INodeEntryFinder nodeEntryFinder;
        private readonly IRowIndexReader<NID> rowIndexReader;
        private readonly IDecoder<NID> nidDecoder;

        public ReadOnlyFolder(
            INodeEntryFinder nodeEntryFinder,
            IRowIndexReader<NID> rowIndexReader,
            IDecoder<NID> nidDecoder)
        {
            this.nodeEntryFinder = nodeEntryFinder;
            this.rowIndexReader = rowIndexReader;
            this.nidDecoder = nidDecoder;
        }

        public Maybe<NID[]> GetNodeIdsForSubFolders(NID folderNodeId)
        {
            var entry =
                nodeEntryFinder.GetEntry(
                    NodePath.OfValue(folderNodeId.ChangeType(Globals.NID_TYPE_HIERARCHY_TABLE)));

            if (entry.HasNoValue)
            {
                return Maybe<NID[]>.NoValue();
            }

            return
                rowIndexReader
                .GetAllRowIds(entry.Value.NodeDataBlockId)
                .Select(rowId => nidDecoder.Decode(rowId.RowId))
                .ToArray();
        }

        public Maybe<NID[]> GetNodeIdsForMessages(NID folderNodeId)
        {
            var entry =
                nodeEntryFinder.GetEntry(
                    NodePath.OfValue(folderNodeId.ChangeType(Globals.NID_TYPE_CONTENTS_TABLE)));

            if (entry.HasNoValue)
            {
                return Maybe<NID[]>.NoValue();
            }

            return
                rowIndexReader
                .GetAllRowIds(entry.Value.NodeDataBlockId)
                .Select(rowId => nidDecoder.Decode(rowId.RowId))
                .ToArray();
        }
    }
}

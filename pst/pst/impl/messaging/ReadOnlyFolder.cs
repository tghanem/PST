using pst.core;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ltp.tc;
using pst.interfaces.messaging;
using pst.interfaces.ndb;
using pst.utilities;
using System.Linq;

namespace pst.impl.messaging
{
    class ReadOnlyFolder : IReadOnlyFolder
    {
        private readonly IRowIndexReader<NID> rowIndexReader;
        private readonly IDecoder<NID> nidDecoder;

        public ReadOnlyFolder(
            IRowIndexReader<NID> rowIndexReader,
            IDecoder<NID> nidDecoder)
        {
            this.rowIndexReader = rowIndexReader;
            this.nidDecoder = nidDecoder;
        }

        public Maybe<NID[]> GetNodeIdsForSubFolders(NID folderNodeId)
        {
            var nodePath =
                NodePath.OfValue(folderNodeId.ChangeType(Constants.NID_TYPE_HIERARCHY_TABLE));

            return
                rowIndexReader
                .GetAllRowIds(nodePath)
                .Select(rowId => nidDecoder.Decode(rowId.RowId))
                .ToArray();
        }

        public Maybe<NID[]> GetNodeIdsForContents(NID folderNodeId)
        {
            var nodePath =
                NodePath.OfValue(folderNodeId.ChangeType(Constants.NID_TYPE_CONTENTS_TABLE));

            return
                rowIndexReader
                .GetAllRowIds(nodePath)
                .Select(rowId => nidDecoder.Decode(rowId.RowId))
                .ToArray();
        }
    }
}

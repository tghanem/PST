using pst.core;
using pst.encodables.ndb;
using pst.interfaces.ltp.pc;
using pst.interfaces.ltp.tc;
using pst.utilities;
using System.Linq;

namespace pst
{
    public class Folder
    {
        private readonly NID nodeId;
        private readonly IPCBasedPropertyReader pcBasedPropertyReader;
        private readonly ITCReader<NID> tableContextReader;

        internal Folder(
            NID nodeId,
            IPCBasedPropertyReader pcBasedPropertyReader,
            ITCReader<NID> tableContextReader)
        {
            this.nodeId = nodeId;
            this.pcBasedPropertyReader = pcBasedPropertyReader;
            this.tableContextReader = tableContextReader;
        }

        public Folder[] GetSubFolders()
        {
            var rowIds =
                tableContextReader.GetAllRowIds(
                    nodeId.ChangeType(Globals.NID_TYPE_HIERARCHY_TABLE));

            return
                rowIds
                .Select(
                    r =>
                    new Folder(r.RowId, pcBasedPropertyReader, tableContextReader))
                .ToArray();
        }

        public Message[] GetMessages()
        {
            var rowIds =
                tableContextReader.GetAllRowIds(
                    nodeId.ChangeType(Globals.NID_TYPE_CONTENTS_TABLE));

            return
                rowIds
                .Select(
                    r =>
                    new Message(r.RowId, pcBasedPropertyReader))
                .ToArray();
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            return pcBasedPropertyReader.ReadProperty(nodeId, propertyTag);
        }
    }
}

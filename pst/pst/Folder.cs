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
        private readonly ITCReader<NID> hierarchyTableReader;

        internal Folder(
            NID nodeId,
            IPCBasedPropertyReader pcBasedPropertyReader,
            ITCReader<NID> hierarchyTableReader)
        {
            this.nodeId = nodeId;
            this.pcBasedPropertyReader = pcBasedPropertyReader;
            this.hierarchyTableReader = hierarchyTableReader;
        }

        public Folder[] GetSubFolders()
        {
            var rowIds =
                hierarchyTableReader.GetAllRowIds(
                    nodeId.ChangeType(Globals.NID_TYPE_HIERARCHY_TABLE));

            return
                rowIds
                .Select(
                    r =>
                    new Folder(r.RowId, pcBasedPropertyReader, hierarchyTableReader))
                .ToArray();
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            return pcBasedPropertyReader.ReadProperty(nodeId, propertyTag);
        }
    }
}

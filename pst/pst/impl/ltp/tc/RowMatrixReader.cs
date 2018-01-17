using pst.core;
using pst.encodables.ltp.hn;
using pst.encodables.ltp.tc;
using pst.encodables.ndb;
using pst.interfaces.ltp.hn;
using pst.interfaces.ltp.tc;
using pst.interfaces.ndb;
using System.Collections.Generic;

namespace pst.impl.ltp.tc
{
    class RowMatrixReader : IRowMatrixReader
    {
        private readonly IHeapOnNodeReader heapOnNodeReader;
        private readonly IRowValuesExtractor rowValuesExtractor;
        private readonly IDataTreeReader dataTreeReader;

        public RowMatrixReader(
            IHeapOnNodeReader heapOnNodeReader,
            IRowValuesExtractor rowValuesExtractor,
            IDataTreeReader dataTreeReader)
        {
            this.rowValuesExtractor = rowValuesExtractor;
            this.heapOnNodeReader = heapOnNodeReader;
            this.dataTreeReader = dataTreeReader;
        }

        public Maybe<TableRow> GetRow(NID[] nodePath, TCROWID rowId)
        {
            var hnHeader = heapOnNodeReader.GetHeapOnNodeHeader(nodePath);

            var userRootHeapItem = heapOnNodeReader.GetHeapItem(nodePath, hnHeader.UserRoot);

            var tcInfo = TCINFO.OfValue(userRootHeapItem);

            var cebStartingOffset = tcInfo.GroupsOffsets[2];

            var rowLength = tcInfo.GroupsOffsets[3];

            var rowMatrixHnid = HNID.OfValue(tcInfo.RowMatrix);

            if (rowMatrixHnid.IsZero)
            {
                return Maybe<TableRow>.NoValue();
            }

            if (rowMatrixHnid.IsHID)
            {
                var heapItem = heapOnNodeReader.GetHeapItem(nodePath, rowMatrixHnid.HID);

                var encodedRows = heapItem.Slice(rowLength);

                var rowValues = rowValuesExtractor.Extract(encodedRows[rowId.RowIndex], tcInfo.ColumnDescriptors, cebStartingOffset);

                return Maybe<TableRow>.OfValue(new TableRow(rowId, rowValues));
            }
            else
            {
                var numberOfRowsPerBlock = (8 * 1024 - 16) / rowLength;

                var blockIndex = rowId.RowIndex / numberOfRowsPerBlock;

                var childNodePath = new List<NID>(nodePath) { rowMatrixHnid.NID };

                var dataBlock = dataTreeReader.Read(childNodePath.ToArray(), blockIndex)[0];

                var encodedRows = dataBlock.Slice(rowLength);

                var rowValues = rowValuesExtractor.Extract(encodedRows[rowId.RowIndex], tcInfo.ColumnDescriptors, cebStartingOffset);

                return Maybe<TableRow>.OfValue(new TableRow(rowId, rowValues));
            }
        }
    }
}

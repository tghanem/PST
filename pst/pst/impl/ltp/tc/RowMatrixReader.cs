using pst.core;
using pst.encodables.ltp.hn;
using pst.encodables.ltp.tc;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ltp.hn;
using pst.interfaces.ltp.tc;
using pst.interfaces.ndb;
using pst.utilities;
using System.Collections.Generic;

namespace pst.impl.ltp.tc
{
    class RowMatrixReader : IRowMatrixReader
    {
        private readonly IHeapOnNodeReader heapOnNodeReader;
        private readonly IRowValuesExtractor rowValuesExtractor;
        private readonly IDecoder<HNID> hnidDecoder;
        private readonly IDecoder<TCINFO> tcinfoDecoder;
        private readonly IExternalDataBlockReader dataBlockReader;

        public RowMatrixReader(
            IHeapOnNodeReader heapOnNodeReader,
            IRowValuesExtractor rowValuesExtractor,
            IDecoder<HNID> hnidDecoder,
            IDecoder<TCINFO> tcinfoDecoder,
            IExternalDataBlockReader dataBlockReader)
        {
            this.rowValuesExtractor = rowValuesExtractor;
            this.heapOnNodeReader = heapOnNodeReader;
            this.tcinfoDecoder = tcinfoDecoder;
            this.hnidDecoder = hnidDecoder;
            this.dataBlockReader = dataBlockReader;
        }

        public Maybe<TableRow> GetRow(NID[] nodePath, TCROWID rowId)
        {
            var hnHeader = heapOnNodeReader.GetHeapOnNodeHeader(nodePath);

            var userRootHeapItem = heapOnNodeReader.GetHeapItem(nodePath, hnHeader.UserRoot);

            var tcInfo = tcinfoDecoder.Decode(userRootHeapItem);

            var cebStartingOffset = tcInfo.GroupsOffsets[2];

            var rowLength = tcInfo.GroupsOffsets[3];

            var rowMatrixHnid = hnidDecoder.Decode(tcInfo.RowMatrix);

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

                var dataBlock = dataBlockReader.Read(childNodePath.ToArray(), blockIndex)[0];

                var encodedRows = dataBlock.Slice(rowLength);

                var rowValues = rowValuesExtractor.Extract(encodedRows[rowId.RowIndex], tcInfo.ColumnDescriptors, cebStartingOffset);

                return Maybe<TableRow>.OfValue(new TableRow(rowId, rowValues));
            }
        }
    }
}

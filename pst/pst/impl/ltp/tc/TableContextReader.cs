using pst.interfaces;
using pst.interfaces.ltp.tc;
using pst.interfaces.ndb;
using System;
using System.Collections.Generic;

namespace pst.impl.ltp.tc
{
    class TableContextReader<TRowId> : ITableContextReader
    {
        private readonly IDecoder<TRowId> rowIdDecoder;
        private readonly IRowIndexReader<TRowId> rowIndexReader;
        private readonly IRowMatrixReader<TRowId> rowMatrixReader;

        public TableContextReader(
            IDecoder<TRowId> rowIdDecoder,
            IRowIndexReader<TRowId> rowIndexReader,
            IRowMatrixReader<TRowId> rowMatrixReader)
        {
            this.rowIdDecoder = rowIdDecoder;
            this.rowIndexReader = rowIndexReader;
            this.rowMatrixReader = rowMatrixReader;
        }

        public TableRow[] GetAllRows(NodePath nodePath)
        {
            var rowIds = rowIndexReader.GetAllRowIds(nodePath);

            var rows = new List<TableRow>();

            Array.ForEach(
                rowIds,
                id =>
                {
                    var row = rowMatrixReader.GetRow(nodePath, rowIdDecoder.Decode(id.RowId));

                    rows.Add(row.Value);
                });

            return rows.ToArray();
        }
    }
}

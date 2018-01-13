using pst.interfaces.ltp.tc;
using pst.interfaces.model;
using System;
using System.Collections.Generic;

namespace pst.impl.ltp.tc
{
    class TableContextReader : ITableContextReader
    {
        private readonly IRowIndexReader rowIndexReader;
        private readonly IRowMatrixReader rowMatrixReader;

        public TableContextReader(
            IRowIndexReader rowIndexReader,
            IRowMatrixReader rowMatrixReader)
        {
            this.rowIndexReader = rowIndexReader;
            this.rowMatrixReader = rowMatrixReader;
        }

        public TableRow[] GetAllRows(NodePath nodePath)
        {
            var rowIds = rowIndexReader.GetAllRowIds(nodePath.AllocatedIds);

            var rows = new List<TableRow>();

            Array.ForEach(
                rowIds,
                id =>
                {
                    var row = rowMatrixReader.GetRow(nodePath.AllocatedIds, id);

                    rows.Add(row.Value);
                });

            return rows.ToArray();
        }
    }
}

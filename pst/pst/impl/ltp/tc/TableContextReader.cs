using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ltp.tc;
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

        public TableRow[] GetAllRows(BID nodeBlockId, BID subnodeBlockId)
        {
            var rowIds = rowIndexReader.GetAllRowIds(nodeBlockId);

            var rows = new List<TableRow>();

            Array.ForEach(
                rowIds,
                id =>
                {
                    var row = rowMatrixReader.GetRow(nodeBlockId, subnodeBlockId, rowIdDecoder.Decode(id.RowId));

                    rows.Add(row.Value);
                });

            return rows.ToArray();
        }
    }
}

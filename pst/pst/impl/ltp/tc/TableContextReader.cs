using pst.interfaces.ltp.tc;
using pst.interfaces.model;
using System.Linq;

namespace pst.impl.ltp.tc
{
    class TableContextReader : ITableContextReader
    {
        private readonly IRowIndexReader rowIndexReader;
        private readonly IRowMatrixReader rowMatrixReader;

        public TableContextReader(IRowIndexReader rowIndexReader, IRowMatrixReader rowMatrixReader)
        {
            this.rowIndexReader = rowIndexReader;
            this.rowMatrixReader = rowMatrixReader;
        }

        public TableRow[] GetAllRows(NodePath nodePath)
        {
            return
                rowIndexReader
                .GetAllRowIds(nodePath.AllocatedIds)
                .Select(id => rowMatrixReader.GetRow(nodePath.AllocatedIds, id).Value)
                .ToArray();
        }
    }
}

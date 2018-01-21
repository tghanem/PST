using pst.core;
using pst.encodables.ndb;
using pst.interfaces.ltp.tc;
using pst.interfaces.messaging;

namespace pst.impl.messaging
{
    class TableContextBasedPropertyReader : ITableContextBasedPropertyReader
    {
        private readonly IRowIndexReader rowIndexReader;
        private readonly IRowMatrixReader rowMatrixReader;
        private readonly IPropertyValueReader propertyValueReader;

        public TableContextBasedPropertyReader(
            IRowIndexReader rowIndexReader,
            IRowMatrixReader rowMatrixReader,
            IPropertyValueReader propertyValueReader)
        {
            this.rowIndexReader = rowIndexReader;
            this.rowMatrixReader = rowMatrixReader;
            this.propertyValueReader = propertyValueReader;
        }

        public Maybe<PropertyValue> Read(NID[] nodePath, int rowId, PropertyTag propertyTag)
        {
            var rowIndex = rowIndexReader.GetRowIndex(nodePath, rowId);

            if (rowIndex.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            var row = rowMatrixReader.GetRow(nodePath, rowIndex.Value);

            if (row.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            if (!row.Value.Values.ContainsKey(propertyTag.Value))
            {
                return Maybe<PropertyValue>.NoValue();
            }

            var propertyValue = row.Value.Values[propertyTag.Value];

            return propertyValueReader.Read(nodePath, propertyTag.Type, propertyValue);
        }
    }
}

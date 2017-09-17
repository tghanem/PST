using pst.core;
using pst.interfaces.ltp;
using pst.interfaces.ltp.tc;
using pst.interfaces.messaging;
using pst.interfaces.ndb;

namespace pst.impl.ltp.tc
{
    class TableContextBasedPropertyReader<TRowId> : ITableContextBasedPropertyReader<TRowId>
    {
        private readonly IRowMatrixReader<TRowId> rowMatrixReader;
        private readonly IPropertyValueProcessor propertyValueProcessor;

        public TableContextBasedPropertyReader(
            IRowMatrixReader<TRowId> rowMatrixReader,
            IPropertyValueProcessor propertyValueProcessor)
        {
            this.rowMatrixReader = rowMatrixReader;
            this.propertyValueProcessor = propertyValueProcessor;
        }

        public Maybe<PropertyValue> ReadProperty(NodePath nodePath, TRowId rowId, PropertyTag propertyTag)
        {
            var row = rowMatrixReader.GetRow(nodePath, rowId);

            if (row.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            if (!row.Value.Values.ContainsKey(propertyTag.Value))
            {
                return Maybe<PropertyValue>.NoValue();
            }

            var propertyValue = row.Value.Values[propertyTag.Value];

            return propertyValueProcessor.Process(nodePath, propertyTag.Type, propertyValue);
        }
    }
}

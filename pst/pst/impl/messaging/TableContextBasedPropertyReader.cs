using pst.core;
using pst.encodables.ltp.tc;
using pst.encodables.ndb;
using pst.interfaces.ltp.tc;
using pst.interfaces.messaging;

namespace pst.impl.messaging
{
    class TableContextBasedPropertyReader : ITableContextBasedPropertyReader
    {
        private readonly IRowMatrixReader rowMatrixReader;
        private readonly IPropertyValueReader propertyValueReader;

        public TableContextBasedPropertyReader(
            IRowMatrixReader rowMatrixReader,
            IPropertyValueReader propertyValueReader)
        {
            this.rowMatrixReader = rowMatrixReader;
            this.propertyValueReader = propertyValueReader;
        }

        public Maybe<PropertyValue> Read(NID[] nodePath, TCROWID rowId, PropertyTag propertyTag)
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

            return propertyValueReader.Read(nodePath, propertyTag.Type, propertyValue);
        }
    }
}

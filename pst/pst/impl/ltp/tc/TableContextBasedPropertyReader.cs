using pst.core;
using pst.encodables.ndb;
using pst.interfaces.ltp;
using pst.interfaces.ltp.tc;
using pst.interfaces.messaging;

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

        public Maybe<PropertyValue> ReadProperty(BID nodeDataBlockId, BID subnodeDataBlockId, TRowId rowId, PropertyTag propertyTag)
        {
            var row = rowMatrixReader.GetRow(nodeDataBlockId, subnodeDataBlockId, rowId);

            if (row.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            if (!row.Value.Values.ContainsKey(propertyTag.Value))
            {
                return Maybe<PropertyValue>.NoValue();
            }

            var propertyValue = row.Value.Values[propertyTag.Value];

            return propertyValueProcessor.Process(nodeDataBlockId, subnodeDataBlockId, propertyTag.Type, propertyValue);
        }
    }
}

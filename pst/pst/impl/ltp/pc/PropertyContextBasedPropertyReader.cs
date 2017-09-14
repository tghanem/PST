using pst.core;
using pst.encodables.ndb;
using pst.interfaces.ltp;
using pst.interfaces.ltp.bth;
using pst.interfaces.messaging;

namespace pst.impl.ltp.pc
{
    class PropertyContextBasedPropertyReader : IPropertyContextBasedPropertyReader
    {
        private readonly IBTreeOnHeapReader<PropertyId> bthReader;
        private readonly IPropertyValueProcessor propertyValueProcessor;

        public PropertyContextBasedPropertyReader(
            IBTreeOnHeapReader<PropertyId> bthReader,
            IPropertyValueProcessor propertyValueProcessor)
        {
            this.bthReader = bthReader;
            this.propertyValueProcessor = propertyValueProcessor;
        }

        public Maybe<PropertyValue> ReadProperty(BID nodeDataBlockId, BID subnodeDataBlockId, PropertyTag propertyTag)
        {
            var dataRecord =
                bthReader.ReadDataRecord(nodeDataBlockId, propertyTag.Id);

            if (dataRecord.HasNoValue)
                return Maybe<PropertyValue>.NoValue();

            return
                propertyValueProcessor.Process(
                    nodeDataBlockId,
                    subnodeDataBlockId,
                    propertyTag.Type,
                    dataRecord.Value.Data.Take(2, 4));
        }
    }
}

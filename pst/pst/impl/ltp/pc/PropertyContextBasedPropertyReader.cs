using pst.core;
using pst.interfaces.ltp.bth;
using pst.interfaces.ltp.pc;
using pst.interfaces.messaging;
using pst.interfaces.ndb;

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

        public Maybe<PropertyValue> ReadProperty(NodePath nodePath, PropertyTag propertyTag)
        {
            var dataRecord =
                bthReader.ReadDataRecord(nodePath, propertyTag.Id);

            if (dataRecord.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return
                propertyValueProcessor.Process(
                    nodePath,
                    propertyTag.Type,
                    dataRecord.Value.Data.Take(2, 4));
        }
    }
}

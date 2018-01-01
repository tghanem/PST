using pst.core;
using pst.interfaces.ltp;
using pst.interfaces.ltp.bth;
using pst.interfaces.ltp.pc;
using pst.interfaces.ndb;

namespace pst.impl.ltp.pc
{
    class PropertyContextBasedPropertyReader : IPropertyContextBasedPropertyReader
    {
        private readonly IBTreeOnHeapReader<PropertyId> bthReader;
        private readonly IPropertyValueReader propertyValueReader;

        public PropertyContextBasedPropertyReader(
            IBTreeOnHeapReader<PropertyId> bthReader,
            IPropertyValueReader propertyValueReader)
        {
            this.bthReader = bthReader;
            this.propertyValueReader = propertyValueReader;
        }

        public Maybe<PropertyValue> Read(NodePath nodePath, PropertyTag propertyTag)
        {
            var dataRecord =
                bthReader.ReadDataRecord(nodePath, propertyTag.Id);

            if (dataRecord.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return
                propertyValueReader.Read(
                    nodePath,
                    propertyTag.Type,
                    dataRecord.Value.Data.Take(2, 4));
        }
    }
}

using pst.core;
using pst.encodables.ltp.bth;
using pst.encodables.ltp.hn;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.ltp.hn;
using pst.interfaces.messaging;
using pst.utilities;
using System;

namespace pst.impl.ltp.pc
{
    class PropertyContext
    {
        private readonly IBTreeKeyFinder<DataRecord, PropertyId> btreeOnHeapKeyFinder;
        private readonly IPropertyTypeMetadataProvider propertyTypeMetadataProvider;
        private readonly IDecoder<PropertyType> propertyTypeDecoder;
        private readonly IHeapOnNodeItemLoader heapOnNodeItemLoader;
        private readonly IDecoder<HID> hidDecoder;
        private readonly IDecoder<NID> nidDecoder;

        public PropertyContext(
            IBTreeKeyFinder<DataRecord, PropertyId> btreeOnHeapKeyFinder,
            IPropertyTypeMetadataProvider propertyTypeMetadataProvider,
            IDecoder<PropertyType> propertyTypeDecoder,
            IHeapOnNodeItemLoader heapOnNodeItemLoader,
            IDecoder<HID> hidDecoder,
            IDecoder<NID> nidDecoder)
        {
            this.hidDecoder = hidDecoder;
            this.nidDecoder = nidDecoder;
            this.propertyTypeDecoder = propertyTypeDecoder;
            this.btreeOnHeapKeyFinder = btreeOnHeapKeyFinder;
            this.heapOnNodeItemLoader = heapOnNodeItemLoader;
            this.propertyTypeMetadataProvider = propertyTypeMetadataProvider;
        }

        public Maybe<PropertyValue> GetPropertyValue(PropertyId propertyId)
        {
            var dataRecord = btreeOnHeapKeyFinder.Find(propertyId);

            var parser = BinaryDataParser.OfValue(dataRecord.Value.Data);

            var propertyType = parser.TakeAndSkip(2, propertyTypeDecoder);

            if (propertyTypeMetadataProvider.IsFixedLength(propertyType))
            {
                var size = propertyTypeMetadataProvider.GetFixedLengthTypeSize(propertyType);

                if (size <= 4)
                {
                    return
                        new PropertyValue(
                            parser.TakeAndSkip(4).Value);
                }
                else
                {
                    var hid =
                        parser.TakeAndSkip(4, hidDecoder);

                    return LoadPropertyValueFromHeapItem(hid);
                }
            }
            else if (propertyTypeMetadataProvider.IsVariableLength(propertyType))
            {
                var hid =
                    parser.TakeAtWithoutChangingStreamPosition(2, 4, hidDecoder);

                if (hid.Type == Globals.NID_TYPE_HID)
                {
                    return LoadPropertyValueFromHeapItem(hid);
                }
                else
                {
                    throw new Exception("Loading data from subnodes is not supported");
                }
            }
            else
            {
                throw new Exception($"Property type {propertyType.Value} is not supported");
            }
        }

        private Maybe<PropertyValue> LoadPropertyValueFromHeapItem(HID itemId)
        {
            var heapItem =
                heapOnNodeItemLoader.Load(itemId);

            if (heapItem.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue<PropertyValue>();
            }

            return
                new PropertyValue(
                    heapItem.Value.Value);
        }
    }
}

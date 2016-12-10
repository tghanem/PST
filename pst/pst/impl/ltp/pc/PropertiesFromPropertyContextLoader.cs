using pst.interfaces.ltp.pc;
using System.Collections.Generic;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces.io;
using pst.interfaces.ltp.hn;
using pst.interfaces.ltp.bth;
using pst.encodables.ltp.bth;
using pst.utilities;
using pst.interfaces;
using pst.interfaces.messaging;
using pst.encodables.ltp.hn;

namespace pst.impl.ltp.pc
{
    class PropertiesFromPropertyContextLoader
        : IPropertiesFromPropertyContextLoader
    {
        private readonly IHeapOnNodeLoader heapOnNodeLoader;
        private readonly IDecoder<HID> hidDecoder;
        private readonly IDecoder<int> int32Decoder;
        private readonly IDecoder<PropertyType> propertyTypeDecoder;
        private readonly IPropertyTypeMetadataProvider propertyTypeMetadataProvider;
        private readonly IBTreeOnHeapLeafKeysEnumerator btreeOnHeapLeafKeysEnumerator;

        public PropertiesFromPropertyContextLoader(
            IHeapOnNodeLoader heapOnNodeLoader,
            IDecoder<HID> hidDecoder,
            IDecoder<int> int32Decoder,
            IDecoder<PropertyType> propertyTypeDecoder,
            IPropertyTypeMetadataProvider propertyTypeMetadataProvider,
            IBTreeOnHeapLeafKeysEnumerator btreeOnHeapLeafKeysEnumerator)
        {
            this.heapOnNodeLoader = heapOnNodeLoader;
            this.hidDecoder = hidDecoder;
            this.int32Decoder = int32Decoder;
            this.propertyTypeDecoder = propertyTypeDecoder;
            this.propertyTypeMetadataProvider = propertyTypeMetadataProvider;
            this.btreeOnHeapLeafKeysEnumerator = btreeOnHeapLeafKeysEnumerator;
        }

        public IDictionary<PropertyId, PropertyValue> Load(
            IDataBlockReader<LBBTEntry> reader,
            IReadOnlyDictionary<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry)
        {
            var heapOnNode =
                heapOnNodeLoader
                .Load(
                    reader,
                    blockIdToEntryMapping,
                    blockEntry);

            var dataRecords =
                btreeOnHeapLeafKeysEnumerator
                .Enumerate(
                    heapOnNode);

            var properties = new Dictionary<PropertyId, PropertyValue>();

            foreach (var dataRecord in dataRecords)
            {
                var parser = BinaryDataParser.OfValue(dataRecord.Key);

                var propertyId = parser.TakeAndSkip(2, int32Decoder);

                var propertyValue = LoadPropertyValue(dataRecord, heapOnNode);

                properties.Add(
                    new PropertyId(propertyId),
                    propertyValue);
            }

            return properties;
        }

        private PropertyValue LoadPropertyValue(DataRecord dataRecord, HeapOnNode heapOnNode)
        {
            var parser = BinaryDataParser.OfValue(dataRecord.Data);

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

                    return new PropertyValue(heapOnNode.GetItem(hid).Value);
                }
            }
            else if (propertyTypeMetadataProvider.IsVariableLength(propertyType))
            {
                var hid =
                    parser.TakeAtWithoutChangingStreamPosition(2, 4, hidDecoder);

                if (hid.Type == Globals.NID_TYPE_HID)
                {
                    return new PropertyValue(heapOnNode.GetItem(hid).Value);
                }
            }

            return new PropertyValue(new byte[0]);
        }
    }
}

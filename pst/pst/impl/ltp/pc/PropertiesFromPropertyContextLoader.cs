using pst.interfaces.ltp.pc;
using System.Collections.Generic;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces.io;
using pst.interfaces.ltp.hn;
using pst.interfaces.ltp.bth;
using pst.utilities;
using pst.interfaces;
using pst.encodables.ltp.hn;
using pst.interfaces.ltp;

namespace pst.impl.ltp.pc
{
    class PropertiesFromPropertyContextLoader
        : IPropertiesFromPropertyContextLoader
    {
        private readonly IHeapOnNodeLoader heapOnNodeLoader;
        private readonly IDecoder<HID> hidDecoder;
        private readonly IDecoder<int> int32Decoder;
        private readonly IPropertyValueLoader propertyValueLoader;
        private readonly IDecoder<PropertyType> propertyTypeDecoder;
        private readonly IBTreeOnHeapLeafKeysEnumerator btreeOnHeapLeafKeysEnumerator;

        public PropertiesFromPropertyContextLoader(
            IHeapOnNodeLoader heapOnNodeLoader,
            IDecoder<HID> hidDecoder,
            IDecoder<int> int32Decoder,
            IPropertyValueLoader propertyValueLoader,
            IDecoder<PropertyType> propertyTypeDecoder,
            IBTreeOnHeapLeafKeysEnumerator btreeOnHeapLeafKeysEnumerator)
        {
            this.heapOnNodeLoader = heapOnNodeLoader;
            this.hidDecoder = hidDecoder;
            this.int32Decoder = int32Decoder;
            this.propertyValueLoader = propertyValueLoader;
            this.propertyTypeDecoder = propertyTypeDecoder;
            this.btreeOnHeapLeafKeysEnumerator = btreeOnHeapLeafKeysEnumerator;
        }

        public Dictionary<PropertyId, PropertyValue> Load(
            IDataBlockReader<LBBTEntry> reader,
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
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
                var propertyIdParser = BinaryDataParser.OfValue(dataRecord.Key);

                var propertyId = propertyIdParser.TakeAndSkip(2, int32Decoder);


                var propertyTypeParser = BinaryDataParser.OfValue(dataRecord.Data);

                var propertyType = propertyTypeParser.TakeAndSkip(2, propertyTypeDecoder);

                var propertyValue =
                    propertyValueLoader
                    .Load(
                        propertyType,
                        propertyTypeParser.TakeAndSkip(4),
                        heapOnNode);


                properties.Add(
                    new PropertyId(propertyId),
                    propertyValue);
            }

            return properties;
        }
    }
}

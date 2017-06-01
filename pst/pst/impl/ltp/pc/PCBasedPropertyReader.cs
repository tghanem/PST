using pst.core;
using pst.encodables.ltp.hn;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.ltp;
using pst.interfaces.ltp.bth;
using pst.interfaces.ltp.hn;
using pst.interfaces.ltp.pc;
using pst.utilities;

namespace pst.impl.ltp.pc
{
    class PCBasedPropertyReader
        : IPCBasedPropertyReader
    {
        private readonly IHeapOnNodeReader heapOnNodeReader;
        private readonly IDecoder<HNID> hnidDecoder;
        private readonly IBTreeOnHeapReader<PropertyId> bthReader;
        private readonly IPropertyTypeMetadataProvider propertyTypeMetadataProvider;

        public PCBasedPropertyReader(
            IHeapOnNodeReader heapOnNodeReader,
            IDecoder<HNID> hnidDecoder,
            IBTreeOnHeapReader<PropertyId> bthReader,
            IPropertyTypeMetadataProvider propertyTypeMetadataProvider)
        {
            this.heapOnNodeReader = heapOnNodeReader;
            this.hnidDecoder = hnidDecoder;
            this.bthReader = bthReader;
            this.propertyTypeMetadataProvider = propertyTypeMetadataProvider;
        }

        public Maybe<PropertyValue> ReadProperty(
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry,
            PropertyTag propertyTag)
        {
            var dataRecord =
                bthReader
                .ReadDataRecord(blockIdToEntryMapping, blockEntry, propertyTag.Id);

            if (dataRecord.HasNoValue)
                return Maybe<PropertyValue>.NoValue();

            if (propertyTypeMetadataProvider.IsFixedLength(propertyTag.Type))
            {
                var size =
                    propertyTypeMetadataProvider
                    .GetFixedLengthTypeSize(propertyTag.Type);

                if (size <= 4)
                {
                    return new PropertyValue(dataRecord.Value.Data);
                }
                else
                {
                    var encodedHNID =
                        BinaryDataParser
                        .OfValue(dataRecord.Value.Data)
                        .TakeAndSkip(2)
                        .Take(4);

                    var hnid = hnidDecoder.Decode(encodedHNID);

                    return
                        new PropertyValue(
                            heapOnNodeReader
                            .GetHeapItem(
                                blockIdToEntryMapping,
                                blockEntry,
                                hnid.HID));
                }
            }
            else if (propertyTypeMetadataProvider.IsVariableLength(propertyTag.Type))
            {
                var encodedHNID =
                    BinaryDataParser
                    .OfValue(dataRecord.Value.Data)
                    .Skip(2)
                    .TakeAndSkip(4);

                var hnid = hnidDecoder.Decode(encodedHNID);

                if (hnid.IsHID)
                {
                    if (hnid.HID.Value == 0)
                    {
                        return Maybe<PropertyValue>.NoValue();
                    }

                    return
                        new PropertyValue(
                            heapOnNodeReader
                            .GetHeapItem(
                                blockIdToEntryMapping,
                                blockEntry,
                                hnid.HID));
                }
            }

            return Maybe<PropertyValue>.NoValue();
        }
    }
}

using pst.core;
using pst.encodables.ltp.hn;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ltp;
using pst.interfaces.ltp.bth;
using pst.interfaces.ltp.hn;
using pst.interfaces.ltp.pc;
using pst.utilities;

namespace pst.impl.ltp.pc
{
    class PCBasedPropertyReader : IPCBasedPropertyReader
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

        public Maybe<PropertyValue> ReadProperty(BID nodeDataBlockId, BID subnodeDataBlockId, PropertyTag propertyTag)
        {
            var dataRecord =
                bthReader.ReadDataRecord(nodeDataBlockId, propertyTag.Id);

            if (dataRecord.HasNoValue)
                return Maybe<PropertyValue>.NoValue();

            if (propertyTypeMetadataProvider.IsFixedLength(propertyTag.Type))
            {
                var size =
                    propertyTypeMetadataProvider.GetFixedLengthTypeSize(propertyTag.Type);

                if (size <= 4)
                {
                    return new PropertyValue(dataRecord.Value.Data.Take(2, 4));
                }
                else
                {
                    var hnid = hnidDecoder.Decode(dataRecord.Value.Data.Take(2, 4));

                    var heapItem = heapOnNodeReader.GetHeapItem(nodeDataBlockId, hnid.HID);

                    return new PropertyValue(heapItem);
                }
            }
            else if (propertyTypeMetadataProvider.IsVariableLength(propertyTag.Type))
            {
                var hnid = hnidDecoder.Decode(dataRecord.Value.Data.Take(2, 4));

                if (hnid.IsHID)
                {
                    if (hnid.HID.Index == 0)
                    {
                        return Maybe<PropertyValue>.OfValue(PropertyValue.Empty);
                    }

                    var heapItem = heapOnNodeReader.GetHeapItem(nodeDataBlockId, hnid.HID);

                    return new PropertyValue(heapItem);
                }
            }
            else if (propertyTag.Type.Value == Globals.PtypObject)
            {
                var hnid = hnidDecoder.Decode(dataRecord.Value.Data.Take(2, 4));

                var heapItem = heapOnNodeReader.GetHeapItem(nodeDataBlockId, hnid.HID);

                return new PropertyValue(heapItem);
            }

            return Maybe<PropertyValue>.NoValue();
        }
    }
}

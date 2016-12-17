using pst.interfaces.ltp;
using pst.encodables.ndb;
using pst.encodables.ndb.blocks.subnode;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.io;
using pst.interfaces.ltp.hn;
using pst.encodables.ltp.hn;
using pst.utilities;

namespace pst.impl.ltp
{
    class PropertyValueLoader : IPropertyValueLoader
    {
        private readonly IPropertyTypeMetadataProvider propertyTypeMetadataProvider;
        private readonly IHeapOnNodeLoader heapOnNodeLoader;
        private readonly IDecoder<HNID> hnidDecoder;

        public PropertyValueLoader(
            IPropertyTypeMetadataProvider propertyTypeMetadataProvider,
            IHeapOnNodeLoader heapOnNodeLoader,
            IDecoder<HNID> hnidDecoder)
        {
            this.propertyTypeMetadataProvider = propertyTypeMetadataProvider;
            this.heapOnNodeLoader = heapOnNodeLoader;
            this.hnidDecoder = hnidDecoder;
        }

        public PropertyValue Load(
            PropertyId id,
            PropertyType propertyType,
            BinaryData encodedValue,
            IDataBlockReader<LBBTEntry> reader,
            IMapper<NID, SLEntry> nidToSLEntryMapping,
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry)
        {
            var heapOnNode =
                heapOnNodeLoader.Load(
                    reader,
                    blockIdToEntryMapping,
                    blockEntry);

            if (propertyTypeMetadataProvider.IsFixedLength(propertyType))
            {
                var size = propertyTypeMetadataProvider.GetFixedLengthTypeSize(propertyType);

                if (size <= 4)
                {
                    return new PropertyValue(
                        encodedValue.Value);
                }
                else
                {
                    var hnid =
                        hnidDecoder.Decode(encodedValue);;

                    return new PropertyValue(
                        heapOnNode.GetItem(hnid.HID).Value);
                }
            }
            else if (propertyTypeMetadataProvider.IsVariableLength(propertyType))
            {
                var hnid =
                    hnidDecoder.Decode(encodedValue);

                if (hnid.IsHID)
                {
                    if (hnid.HID.Value == 0)
                    {
                        return new PropertyValue(new byte[0]);
                    }

                    return new PropertyValue(
                        heapOnNode.GetItem(hnid.HID).Value);
                }
            }

            return new PropertyValue(new byte[0]);
        }
    }
}

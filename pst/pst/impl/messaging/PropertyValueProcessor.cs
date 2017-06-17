using pst.encodables.ltp.hn;
using pst.encodables.ndb;
using pst.encodables.ndb.blocks.data;
using pst.interfaces;
using pst.interfaces.io;
using pst.interfaces.ltp;
using pst.interfaces.ltp.hn;
using pst.interfaces.messaging;
using pst.interfaces.ndb;
using pst.utilities;
using System;
using System.IO;
using System.Linq;

namespace pst.impl.messaging
{
    class PropertyValueProcessor : IPropertyValueProcessor
    {
        private readonly IDecoder<HNID> hnidDecoder;
        private readonly IDecoder<ExternalDataBlock> externalDataBlockDecoder;

        private readonly IHeapOnNodeReader heapOnNodeReader;
        private readonly ISubNodesEnumerator subnodesEnumerator;
        private readonly IDataTreeLeafBIDsEnumerator dataTreeLeafBlockIdsEnumerator;
        private readonly IPropertyTypeMetadataProvider propertyTypeMetadataProvider;

        private readonly IDataBlockReader dataBlockReader;

        public PropertyValueProcessor(
            IDecoder<HNID> hnidDecoder,
            IDecoder<ExternalDataBlock> externalDataBlockDecoder,
            IHeapOnNodeReader heapOnNodeReader,
            ISubNodesEnumerator subnodesEnumerator,
            IDataTreeLeafBIDsEnumerator dataTreeLeafBlockIdsEnumerator,
            IPropertyTypeMetadataProvider propertyTypeMetadataProvider,
            IDataBlockReader dataBlockReader)
        {
            this.hnidDecoder = hnidDecoder;
            this.externalDataBlockDecoder = externalDataBlockDecoder;
            this.heapOnNodeReader = heapOnNodeReader;
            this.subnodesEnumerator = subnodesEnumerator;
            this.dataTreeLeafBlockIdsEnumerator = dataTreeLeafBlockIdsEnumerator;
            this.propertyTypeMetadataProvider = propertyTypeMetadataProvider;
            this.dataBlockReader = dataBlockReader;
        }

        public PropertyValue Process(BID dataBlockId, BID subnodeDataBlockId, PropertyType propertyType, BinaryData propertyValue)
        {
            if (propertyTypeMetadataProvider.IsFixedLength(propertyType))
            {
                var size =
                    propertyTypeMetadataProvider.GetFixedLengthTypeSize(propertyType);

                if (size <= 4)
                {
                    return new PropertyValue(propertyValue);
                }
                else
                {
                    var hnid = hnidDecoder.Decode(propertyValue);

                    var heapItem = heapOnNodeReader.GetHeapItem(dataBlockId, hnid.HID);

                    return new PropertyValue(heapItem);
                }
            }
            else if (propertyTypeMetadataProvider.IsVariableLength(propertyType))
            {
                var hnid = hnidDecoder.Decode(propertyValue);

                if (hnid.IsHID)
                {
                    if (hnid.HID.Index == 0)
                    {
                        return PropertyValue.Empty;
                    }

                    var heapItem = heapOnNodeReader.GetHeapItem(dataBlockId, hnid.HID);

                    return new PropertyValue(heapItem);
                }
                else if (hnid.IsNID)
                {
                    var subnodes =
                        subnodesEnumerator.Enumerate(subnodeDataBlockId);

                    var subnodeEntry =
                        subnodes.First(s => s.LocalSubnodeId.Value == hnid.NID.Value);

                    var dataBlockIds =
                        dataTreeLeafBlockIdsEnumerator.Enumerate(subnodeEntry.DataBlockId);

                    var stream = new MemoryStream();

                    Array.ForEach(
                        dataBlockIds,
                        id =>
                        {
                            var externalDataBlock =
                                dataBlockReader.Read(id);

                            var decodedExternalDataBlock =
                                externalDataBlockDecoder.Decode(externalDataBlock);

                            stream.Write(decodedExternalDataBlock.Data, 0, decodedExternalDataBlock.Data.Length);
                        });

                    return new PropertyValue(BinaryData.OfValue(stream.ToArray()));
                }
            }
            else if (propertyType.Value == Globals.PtypObject)
            {
                var hnid = hnidDecoder.Decode(propertyValue);

                var heapItem = heapOnNodeReader.GetHeapItem(dataBlockId, hnid.HID);

                return new PropertyValue(heapItem);
            }

            return PropertyValue.Empty;
        }
    }
}

using pst.core;
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
        private readonly IDataBlockReader dataBlockReader;
        private readonly IDataBlockEntryFinder dataBlockEntryFinder;
        private readonly IHeapOnNodeReader heapOnNodeReader;
        private readonly INodeEntryFinder nodeEntryFinder;
        private readonly IPropertyTypeMetadataProvider propertyTypeMetadataProvider;

        public PropertyValueProcessor(
            IDecoder<HNID> hnidDecoder,
            IDecoder<ExternalDataBlock> externalDataBlockDecoder,
            IDataBlockReader dataBlockReader,
            IDataBlockEntryFinder dataBlockEntryFinder,
            INodeEntryFinder nodeEntryFinder,
            IHeapOnNodeReader heapOnNodeReader,
            IPropertyTypeMetadataProvider propertyTypeMetadataProvider)
        {
            this.hnidDecoder = hnidDecoder;
            this.externalDataBlockDecoder = externalDataBlockDecoder;
            this.heapOnNodeReader = heapOnNodeReader;
            this.nodeEntryFinder = nodeEntryFinder;
            this.dataBlockEntryFinder = dataBlockEntryFinder;
            this.propertyTypeMetadataProvider = propertyTypeMetadataProvider;
            this.dataBlockReader = dataBlockReader;
        }

        public PropertyValue Process(NodePath nodePath, PropertyType propertyType, BinaryData propertyValue)
        {
            if (propertyTypeMetadataProvider.IsFixedLength(propertyType))
            {
                var size =
                    propertyTypeMetadataProvider.GetFixedLengthTypeSize(propertyType);

                if (size <= 4)
                {
                    return new PropertyValue(propertyValue);
                }

                var hnid = hnidDecoder.Decode(propertyValue);

                var heapItem = heapOnNodeReader.GetHeapItem(nodePath, hnid.HID);

                return new PropertyValue(heapItem);
            }

            if (propertyTypeMetadataProvider.IsMultiValueFixedLength(propertyType))
            {
                return new PropertyValue(propertyValue);
            }

            if (propertyTypeMetadataProvider.IsVariableLength(propertyType) ||
                propertyTypeMetadataProvider.IsMultiValueVariableLength(propertyType))
            {
                var hnid = hnidDecoder.Decode(propertyValue);

                var value = GetHNIDBinaryData(nodePath, hnid);

                if (value.HasNoValue)
                {
                    return PropertyValue.Empty;
                }

                return new PropertyValue(value.Value);
            }

            if (propertyType.Value == Constants.PtypObject)
            {
                var hnid = hnidDecoder.Decode(propertyValue);

                var heapItem = heapOnNodeReader.GetHeapItem(nodePath, hnid.HID);

                return new PropertyValue(heapItem);
            }

            return PropertyValue.Empty;
        }

        private Maybe<BinaryData> GetHNIDBinaryData(NodePath nodePath, HNID hnid)
        {
            if (hnid.IsHID)
            {
                if (hnid.HID.Index == 0)
                {
                    return Maybe<BinaryData>.NoValue();
                }

                var heapItem = heapOnNodeReader.GetHeapItem(nodePath, hnid.HID);

                return Maybe<BinaryData>.OfValue(heapItem);
            }

            if (hnid.IsNID)
            {
                var nodeEntry = nodeEntryFinder.GetEntry(nodePath);

                var subnodeEntry =
                    nodeEntry.Value.ChildNodes.First(s => s.LocalSubnodeId.Value == hnid.NID.Value);

                var dataBlockTree =
                    dataBlockEntryFinder.Find(subnodeEntry.DataBlockId);

                if (dataBlockTree.Value.ChildBlockIds.HasValueAnd(childBlockIds => childBlockIds.Length > 0))
                {
                    return ReadSubnodeBinaryData(dataBlockTree.Value.ChildBlockIds.Value);
                }

                return ReadSubnodeBinaryData(subnodeEntry.DataBlockId);
            }

            return Maybe<BinaryData>.NoValue();
        }

        private Maybe<BinaryData> ReadSubnodeBinaryData(params BID[] dataBlockIds)
        {
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

            return Maybe<BinaryData>.OfValue(BinaryData.OfValue(stream.ToArray()));
        }
    }
}

using pst.core;
using pst.encodables.ltp.hn;
using pst.encodables.ndb;
using pst.encodables.ndb.blocks.data;
using pst.interfaces;
using pst.interfaces.ltp;
using pst.interfaces.ltp.hn;
using pst.interfaces.ndb;
using pst.utilities;
using System;
using System.IO;
using System.Linq;

namespace pst.impl.ltp
{
    class PropertyValueReader : IPropertyValueReader
    {
        private readonly IDecoder<HNID> hnidDecoder;
        private readonly IDecoder<ExternalDataBlock> externalDataBlockDecoder;
        private readonly IDataBlockReader dataBlockReader;
        private readonly IDataBlockEntryFinder dataBlockEntryFinder;
        private readonly IHeapOnNodeReader heapOnNodeReader;
        private readonly INodeEntryFinder nodeEntryFinder;

        public PropertyValueReader(
            IDecoder<HNID> hnidDecoder,
            IDecoder<ExternalDataBlock> externalDataBlockDecoder,
            IDataBlockReader dataBlockReader,
            IDataBlockEntryFinder dataBlockEntryFinder,
            INodeEntryFinder nodeEntryFinder,
            IHeapOnNodeReader heapOnNodeReader)
        {
            this.hnidDecoder = hnidDecoder;
            this.externalDataBlockDecoder = externalDataBlockDecoder;
            this.heapOnNodeReader = heapOnNodeReader;
            this.nodeEntryFinder = nodeEntryFinder;
            this.dataBlockEntryFinder = dataBlockEntryFinder;
            this.dataBlockReader = dataBlockReader;
        }

        public PropertyValue Read(NID[] nodePath, PropertyType propertyType, BinaryData propertyValue)
        {
            if (propertyType.IsFixedLength())
            {
                var size = propertyType.GetFixedLengthTypeSize();

                if (size <= 4)
                {
                    return new PropertyValue(propertyValue);
                }

                var hnid = hnidDecoder.Decode(propertyValue);

                var heapItem = heapOnNodeReader.GetHeapItem(nodePath, hnid.HID);

                return new PropertyValue(heapItem);
            }

            if (propertyType.IsMultiValueFixedLength() || propertyType.IsVariableLength() || propertyType.IsMultiValueVariableLength())
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

        private Maybe<BinaryData> GetHNIDBinaryData(NID[] nodePath, HNID hnid)
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

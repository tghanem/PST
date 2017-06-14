using pst.core;
using pst.encodables.ltp.hn;
using pst.encodables.ndb;
using pst.encodables.ndb.blocks.data;
using pst.interfaces;
using pst.interfaces.io;
using pst.interfaces.ltp;
using pst.interfaces.ltp.bth;
using pst.interfaces.ltp.hn;
using pst.interfaces.ltp.pc;
using pst.interfaces.ndb;
using pst.utilities;
using System;
using System.IO;
using System.Linq;

namespace pst.impl.ltp.pc
{
    class PCBasedPropertyReader : IPCBasedPropertyReader
    {
        private readonly IDecoder<HNID> hnidDecoder;
        private readonly IHeapOnNodeReader heapOnNodeReader;
        private readonly IBTreeOnHeapReader<PropertyId> bthReader;
        private readonly ISubNodesEnumerator subnodesEnumerator;
        private readonly IDataTreeLeafBIDsEnumerator dataTreeLeafBlockIdsEnumerator;
        private readonly IPropertyTypeMetadataProvider propertyTypeMetadataProvider;

        private readonly IDataBlockReader dataBlockReader;
        private readonly IDecoder<ExternalDataBlock> externalDataBlockDecoder;

        public PCBasedPropertyReader(
            IDecoder<HNID> hnidDecoder,
            IDecoder<ExternalDataBlock> externalDataBlockDecoder,
            IDataBlockReader dataBlockReader,
            IHeapOnNodeReader heapOnNodeReader,
            ISubNodesEnumerator subnodesEnumerator,
            IDataTreeLeafBIDsEnumerator dataTreeLeafBlockIdsEnumerator,
            IPropertyTypeMetadataProvider propertyTypeMetadataProvider,
            IBTreeOnHeapReader<PropertyId> bthReader)
        {
            this.bthReader = bthReader;
            this.hnidDecoder = hnidDecoder;
            this.heapOnNodeReader = heapOnNodeReader;
            this.subnodesEnumerator = subnodesEnumerator;
            this.dataTreeLeafBlockIdsEnumerator = dataTreeLeafBlockIdsEnumerator;
            this.propertyTypeMetadataProvider = propertyTypeMetadataProvider;

            this.dataBlockReader = dataBlockReader;
            this.externalDataBlockDecoder = externalDataBlockDecoder;
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

using pst.core;
using pst.encodables.ltp.hn;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ltp.hn;
using pst.interfaces.messaging;
using pst.interfaces.ndb;
using pst.utilities;
using System.Collections.Generic;

namespace pst.impl.messaging
{
    class PropertyValueReader : IPropertyValueReader
    {
        private readonly IDecoder<HNID> hnidDecoder;
        private readonly IHeapOnNodeReader heapOnNodeReader;
        private readonly IExternalDataBlockReader externalDataBlockReader;

        public PropertyValueReader(
            IDecoder<HNID> hnidDecoder,
            IHeapOnNodeReader heapOnNodeReader,
            IExternalDataBlockReader externalDataBlockReader)
        {
            this.hnidDecoder = hnidDecoder;
            this.heapOnNodeReader = heapOnNodeReader;
            this.externalDataBlockReader = externalDataBlockReader;
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
                var subnodePath = new List<NID>(nodePath) { hnid.NID };

                var generator = BinaryDataGenerator.New();

                foreach (var block in externalDataBlockReader.Read(subnodePath.ToArray(), Maybe<int>.NoValue()))
                {
                    generator.Append(block);
                }

                return generator.GetData();
            }

            return Maybe<BinaryData>.NoValue();
        }
    }
}

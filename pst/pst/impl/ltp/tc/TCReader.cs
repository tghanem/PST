using pst.core;
using pst.encodables.ltp.hn;
using pst.encodables.ltp.tc;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ltp;
using pst.interfaces.ltp.hn;
using pst.interfaces.ltp.tc;
using System;
using System.Collections.Generic;

namespace pst.impl.ltp.tc
{
    class TCReader<TRowId> : ITCReader<TRowId>
    {
        private readonly IDecoder<HID> hidDecoder;
        private readonly IDecoder<HNID> hnidDecoder;
        private readonly IDecoder<TRowId> rowIdDecoder;
        private readonly IHeapOnNodeReader heapOnNodeReader;
        private readonly IRowIndexReader<TRowId> rowIndexReader;
        private readonly IRowMatrixReader<TRowId> rowMatrixReader;        
        private readonly IPropertyTypeMetadataProvider propertyTypeMetadataProvider;

        public TCReader(
            IDecoder<HID> hidDecoder,
            IDecoder<HNID> hnidDecoder,
            IDecoder<TRowId> rowIdDecoder,
            IHeapOnNodeReader heapOnNodeReader,
            IRowIndexReader<TRowId> rowIndexReader,
            IRowMatrixReader<TRowId> rowMatrixReader,
            IPropertyTypeMetadataProvider propertyTypeMetadataProvider)
        {
            this.hidDecoder = hidDecoder;
            this.hnidDecoder = hnidDecoder;
            this.rowIdDecoder = rowIdDecoder;
            this.heapOnNodeReader = heapOnNodeReader;
            this.rowIndexReader = rowIndexReader;
            this.rowMatrixReader = rowMatrixReader;
            this.propertyTypeMetadataProvider = propertyTypeMetadataProvider;
        }

        public TCROWID[] GetAllRowIds(BID nodeBlockId)
        {
            return rowIndexReader.GetAllRowIds(nodeBlockId);
        }

        public TableRow[] GetAllRows(BID nodeBlockId, BID subnodeBlockId)
        {
            var rowIds = GetAllRowIds(nodeBlockId);

            var rows = new List<TableRow>();

            Array.ForEach(
                rowIds,
                id =>
                {
                    var row = GetRow(nodeBlockId, subnodeBlockId, rowIdDecoder.Decode(id.RowId));

                    rows.Add(row.Value);
                });

            return rows.ToArray();
        }

        public Maybe<TableRow> GetRow(BID nodeBlockId, BID subnodeBlockId, TRowId rowId)
        {
            return rowMatrixReader.GetRow(nodeBlockId, subnodeBlockId, rowId);
        }

        public Maybe<PropertyValue> ReadProperty(BID nodeBlockId, BID subnodeBlockId, TRowId rowId, PropertyTag propertyTag)
        {
            var row = GetRow(nodeBlockId, subnodeBlockId, rowId);

            if (row.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            if (!row.Value.Values.ContainsKey(propertyTag.Value))
            {
                return Maybe<PropertyValue>.NoValue();
            }

            var propertyValue = row.Value.Values[propertyTag.Value];

            if (propertyTypeMetadataProvider.IsFixedLength(propertyTag.Type))
            {
                var size =
                    propertyTypeMetadataProvider.GetFixedLengthTypeSize(propertyTag.Type);

                if (size <= 8)
                {
                    return new PropertyValue(propertyValue);
                }
                else
                {
                    var hid = hidDecoder.Decode(propertyValue);

                    var heapItem = heapOnNodeReader.GetHeapItem(nodeBlockId, hid);

                    return new PropertyValue(heapItem);
                }
            }
            else if (propertyTypeMetadataProvider.IsVariableLength(propertyTag.Type))
            {
                var hnid =
                    hnidDecoder.Decode(propertyValue);

                if (hnid.IsHID)
                {
                    if (hnid.HID.Index == 0)
                    {
                        return Maybe<PropertyValue>.OfValue(PropertyValue.Empty);
                    }

                    var heapItem = heapOnNodeReader.GetHeapItem(nodeBlockId, hnid.HID);

                    return new PropertyValue(heapItem);
                }
            }

            return Maybe<PropertyValue>.NoValue();
        }
    }
}

using pst.core;
using pst.encodables.ltp.tc;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ltp.tc;
using pst.interfaces.messaging;
using System;
using System.Collections.Generic;

namespace pst.impl.ltp.tc
{
    class TableContextReader<TRowId> : ITableContextReader<TRowId>
    {
        private readonly IDecoder<TRowId> rowIdDecoder;
        private readonly IRowIndexReader<TRowId> rowIndexReader;
        private readonly IRowMatrixReader<TRowId> rowMatrixReader;
        private readonly IPropertyValueProcessor propertyValueProcessor;

        public TableContextReader(
            IDecoder<TRowId> rowIdDecoder,
            IRowIndexReader<TRowId> rowIndexReader,
            IRowMatrixReader<TRowId> rowMatrixReader,
            IPropertyValueProcessor propertyValueProcessor)
        {
            this.rowIdDecoder = rowIdDecoder;
            this.rowIndexReader = rowIndexReader;
            this.rowMatrixReader = rowMatrixReader;
            this.propertyValueProcessor = propertyValueProcessor;
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
                    var row = rowMatrixReader.GetRow(nodeBlockId, subnodeBlockId, rowIdDecoder.Decode(id.RowId));

                    rows.Add(row.Value);
                });

            return rows.ToArray();
        }

        public Maybe<PropertyValue> ReadProperty(BID nodeBlockId, BID subnodeBlockId, TRowId rowId, PropertyTag propertyTag)
        {
            var row = rowMatrixReader.GetRow(nodeBlockId, subnodeBlockId, rowId);

            if (row.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            if (!row.Value.Values.ContainsKey(propertyTag.Value))
            {
                return Maybe<PropertyValue>.NoValue();
            }

            var propertyValue = row.Value.Values[propertyTag.Value];

            return propertyValueProcessor.Process(nodeBlockId, subnodeBlockId, propertyTag.Type, propertyValue);
        }
    }
}

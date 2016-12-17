using pst.interfaces.ltp.tc;
using pst.encodables.ndb;
using pst.encodables.ndb.blocks.subnode;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.io;
using System.Collections.Generic;
using pst.interfaces.ltp.hn;
using pst.interfaces.ltp;

namespace pst.impl.ltp.tc
{
    class PropertiesFromTableContextRowLoader : IPropertiesFromTableContextRowLoader
    {
        private readonly IPropertyValueLoader propertyValueLoader;
        private readonly IHeapOnNodeLoader heapOnNodeLoader;

        public PropertiesFromTableContextRowLoader(
            IPropertyValueLoader propertyValueLoader,
            IHeapOnNodeLoader heapOnNodeLoader)
        {
            this.propertyValueLoader = propertyValueLoader;
            this.heapOnNodeLoader = heapOnNodeLoader;
        }

        public IDictionary<PropertyId, PropertyValue> Load(
            IDataBlockReader<LBBTEntry> reader,
            IMapper<NID, SLEntry> nidToSLEntryMapping,
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry,
            TableRow tableRow)
        {
            var heapOnNode =
                heapOnNodeLoader.Load(
                    reader,
                    blockIdToEntryMapping,
                    blockEntry);

            var dictionary = new Dictionary<PropertyId, PropertyValue>();

            foreach (var columnTagWithValue in tableRow.Values)
            {
                var propertyId =
                    PropertyId.OfValue(columnTagWithValue.Key >> 16);

                var propertyType =
                    PropertyType.OfValue(columnTagWithValue.Key & 0x00FF);

                var propertyValue =
                    propertyValueLoader.Load(
                        propertyId,
                        propertyType,
                        columnTagWithValue.Value,
                        reader,
                        nidToSLEntryMapping,
                        blockIdToEntryMapping,
                        blockEntry);

                dictionary.Add(propertyId, propertyValue);
            }

            return dictionary;
        }
    }
}

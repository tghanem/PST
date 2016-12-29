using pst.interfaces.ltp.tc;
using pst.encodables.ndb;
using pst.encodables.ndb.blocks.subnode;
using pst.encodables.ndb.btree;
using pst.interfaces;
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

        public Dictionary<PropertyId, PropertyValue> Load(
            HeapOnNode heapOnNode,
            TableRow tableRow)
        {
            var dictionary = new Dictionary<PropertyId, PropertyValue>();

            foreach (var columnTagWithValue in tableRow.Values)
            {
                var propertyId =
                    PropertyId.OfValue(columnTagWithValue.Key >> 16);

                var propertyType =
                    PropertyType.OfValue(columnTagWithValue.Key & 0x00FF);

                var propertyValue =
                    propertyValueLoader
                    .Load(
                        propertyType,
                        columnTagWithValue.Value,
                        heapOnNode);

                dictionary.Add(propertyId, propertyValue);
            }

            return dictionary;
        }
    }
}

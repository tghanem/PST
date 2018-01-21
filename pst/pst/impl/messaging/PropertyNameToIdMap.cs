using pst.core;
using pst.encodables.messaging;
using pst.interfaces.ltp;
using pst.interfaces.messaging;
using pst.interfaces.model;
using pst.utilities;
using System;
using System.Text;

namespace pst.impl.messaging
{
    class PropertyNameToIdMap : IPropertyNameToIdMap
    {
        private static readonly ObjectPath MapObjectPath = new ObjectPath(new[] { Constants.NID_NAME_TO_ID_MAP });

        private readonly IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader;

        public PropertyNameToIdMap(IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader)
        {
            this.propertyContextBasedPropertyReader = propertyContextBasedPropertyReader;
        }

        public Maybe<PropertyId> GetPropertyId(Guid propertySet, int numericalId)
        {
            var entryStream = propertyContextBasedPropertyReader.Read(MapObjectPath.Ids, MAPIProperties.PidTagNameidStreamEntry);

            if (entryStream.HasNoValue)
            {
                return Maybe<PropertyId>.NoValue();
            }

            var entriesCount = entryStream.Value.Value.Length / 8;

            for (var i = 0; i < entriesCount; i++)
            {
                var entry = NAMEID.OfValue(entryStream.Value.Value.Take(i * 8, 8));

                if (entry.Type == 0)
                {
                    if (entry.PropertyId == numericalId)
                    {
                        return Maybe<PropertyId>.OfValue(new PropertyId(entry.PropertyIndex + 0x8000));
                    }
                }
            }

            return Maybe<PropertyId>.NoValue();
        }

        public Maybe<PropertyId> GetPropertyId(Guid propertySet, string propertyName)
        {
            var entryStream = propertyContextBasedPropertyReader.Read(MapObjectPath.Ids, MAPIProperties.PidTagNameidStreamEntry);

            var stringStream = propertyContextBasedPropertyReader.Read(MapObjectPath.Ids, MAPIProperties.PidTagNameidStreamString);

            if (entryStream.HasNoValue || stringStream.HasNoValue)
            {
                return Maybe<PropertyId>.NoValue();
            }

            var entriesCount = entryStream.Value.Value.Length / 8;

            for (var i = 0; i < entriesCount; i++)
            {
                var entry = NAMEID.OfValue(entryStream.Value.Value.Take(i * 8, 8));

                if (entry.Type == 1)
                {
                    var length = stringStream.Value.Value.Take(entry.PropertyId, 4).ToInt32();

                    var value = stringStream.Value.Value.Take(entry.PropertyId + 4, length);

                    var name = Encoding.Unicode.GetString(value);

                    if (name == propertyName)
                    {
                        return Maybe<PropertyId>.OfValue(new PropertyId(entry.PropertyIndex + 0x8000));
                    }
                }
            }

            return Maybe<PropertyId>.NoValue();
        }
    }
}

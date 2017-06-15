using pst.core;
using pst.encodables.messaging;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.ltp;
using pst.interfaces.ltp.pc;
using pst.utilities;
using System;
using System.Text;

namespace pst.impl.messaging
{
    class PropertyNameToIdMap : IPropertyNameToIdMap
    {
        private readonly IDecoder<NAMEID> nameIdDecoder;
        private readonly IPCBasedPropertyReader pcBasedPropertyReader;
        private readonly IMapper<NID, LNBTEntry> nidToLNBTEntryMapper;

        public PropertyNameToIdMap(
            IDecoder<NAMEID> nameIdDecoder,
            IPCBasedPropertyReader pcBasedPropertyReader,
            IMapper<NID, LNBTEntry> nidToLNBTEntryMapper)
        {
            this.nameIdDecoder = nameIdDecoder;
            this.pcBasedPropertyReader = pcBasedPropertyReader;
            this.nidToLNBTEntryMapper = nidToLNBTEntryMapper;
        }

        public Maybe<PropertyId> GetPropertyId(Guid propertySet, int numericalId)
        {
            var lnbtEntryForNameToIdMap =
                nidToLNBTEntryMapper.Map(Globals.NID_NAME_TO_ID_MAP);

            var entryStream =
                pcBasedPropertyReader.ReadProperty(
                    lnbtEntryForNameToIdMap.DataBlockId,
                    lnbtEntryForNameToIdMap.SubnodeBlockId,
                    MAPIProperties.PidTagNameidStreamEntry);

            if (entryStream.HasNoValue)
            {
                return Maybe<PropertyId>.NoValue();
            }

            var entriesCount = entryStream.Value.Value.Length / 8;

            for (var i = 0; i < entriesCount; i++)
            {
                var entry = nameIdDecoder.Decode(entryStream.Value.Value.Take(i * 8, 8));

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
            var lnbtEntryForNameToIdMap =
                nidToLNBTEntryMapper.Map(Globals.NID_NAME_TO_ID_MAP);

            var entryStream =
                pcBasedPropertyReader.ReadProperty(
                    lnbtEntryForNameToIdMap.DataBlockId,
                    lnbtEntryForNameToIdMap.SubnodeBlockId,
                    MAPIProperties.PidTagNameidStreamEntry);

            var stringStream =
                pcBasedPropertyReader.ReadProperty(
                    lnbtEntryForNameToIdMap.DataBlockId,
                    lnbtEntryForNameToIdMap.SubnodeBlockId,
                    MAPIProperties.PidTagNameidStreamString);

            if (entryStream.HasNoValue || stringStream.HasNoValue)
            {
                return Maybe<PropertyId>.NoValue();
            }

            var entriesCount = entryStream.Value.Value.Length / 8;

            for (var i = 0; i < entriesCount; i++)
            {
                var entry = nameIdDecoder.Decode(entryStream.Value.Value.Take(i * 8, 8));

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

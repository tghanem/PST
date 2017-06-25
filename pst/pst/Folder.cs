using pst.core;
using pst.encodables;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.ltp.pc;
using pst.interfaces.ltp.tc;
using pst.interfaces.ndb;
using pst.utilities;
using System.Linq;
using pst.interfaces.ltp;

namespace pst
{
    public class Folder
    {
        private readonly NID nodeId;
        private readonly IDecoder<NID> nidDecoder;
        private readonly ITableContextReader<NID> tableContextReader;
        private readonly ITableContextReader<Tag> tagBasedTableContextReader;
        private readonly ISubNodesEnumerator subnodesEnumerator;
        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly IPCBasedPropertyReader pcBasedPropertyReader;        
        private readonly IMapper<NID, LNBTEntry> nidToLNBTEntryMapper;

        internal Folder(
            NID nodeId,
            IDecoder<NID> nidDecoder,
            ITableContextReader<NID> tableContextReader,
            ITableContextReader<Tag> tagBasedTableContextReader,
            ISubNodesEnumerator subnodesEnumerator,
            IPropertyNameToIdMap propertyNameToIdMap,
            IPCBasedPropertyReader pcBasedPropertyReader,
            IMapper<NID, LNBTEntry> nidToLNBTEntryMapper)
        {
            this.nodeId = nodeId;
            this.tableContextReader = tableContextReader;
            this.nidDecoder = nidDecoder;
            this.tagBasedTableContextReader = tagBasedTableContextReader;
            this.subnodesEnumerator = subnodesEnumerator;
            this.propertyNameToIdMap = propertyNameToIdMap;
            this.pcBasedPropertyReader = pcBasedPropertyReader;            
            this.nidToLNBTEntryMapper = nidToLNBTEntryMapper;
        }

        public Folder[] GetSubFolders()
        {
            var lnbtEntry =
                nidToLNBTEntryMapper.Map(nodeId.ChangeType(Globals.NID_TYPE_HIERARCHY_TABLE));

            var rowIds =
                tableContextReader.GetAllRowIds(lnbtEntry.DataBlockId);

            return
                rowIds
                .Select(
                    r =>
                    new Folder(
                        nidDecoder.Decode(r.RowId),
                        nidDecoder,
                        tableContextReader,                        
                        tagBasedTableContextReader,
                        subnodesEnumerator,
                        propertyNameToIdMap, 
                        pcBasedPropertyReader,                        
                        nidToLNBTEntryMapper))
                .ToArray();
        }

        public Message[] GetMessages()
        {
            var lnbtEntry =
                nidToLNBTEntryMapper.Map(nodeId.ChangeType(Globals.NID_TYPE_CONTENTS_TABLE));

            var rowIds =
                tableContextReader.GetAllRowIds(lnbtEntry.DataBlockId);

            return
                rowIds
                .Select(
                    r =>
                    {
                        var messageNodeId = nidDecoder.Decode(r.RowId);

                        var lnbtEntryForMessage = nidToLNBTEntryMapper.Map(messageNodeId);

                        return
                            new Message(
                                lnbtEntryForMessage.DataBlockId,
                                lnbtEntryForMessage.SubnodeBlockId,
                                nidDecoder,
                                tableContextReader,
                                tagBasedTableContextReader,
                                subnodesEnumerator,
                                propertyNameToIdMap, 
                                pcBasedPropertyReader);
                    })
                .ToArray();
        }

        public Maybe<PropertyValue> GetProperty(NumericalPropertyTag propertyTag)
        {
            var propertyId = propertyNameToIdMap.GetPropertyId(propertyTag.Set, propertyTag.Id);

            if (propertyId.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return GetProperty(new PropertyTag(propertyId.Value, propertyTag.Type));
        }

        public Maybe<PropertyValue> GetProperty(StringPropertyTag propertyTag)
        {
            var propertyId = propertyNameToIdMap.GetPropertyId(propertyTag.Set, propertyTag.Name);

            if (propertyId.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return GetProperty(new PropertyTag(propertyId.Value, propertyTag.Type));
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            var lnbtEntry =
                nidToLNBTEntryMapper.Map(nodeId);

            return
                pcBasedPropertyReader.ReadProperty(
                    lnbtEntry.DataBlockId,
                    lnbtEntry.SubnodeBlockId,
                    propertyTag);
        }
    }
}

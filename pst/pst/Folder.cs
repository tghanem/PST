using pst.core;
using pst.encodables;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.ltp;
using pst.interfaces.ltp.tc;
using pst.interfaces.ndb;
using pst.utilities;
using System.Linq;

namespace pst
{
    public class Folder
    {
        private readonly NID nodeId;
        private readonly IDecoder<NID> nidDecoder;
        private readonly IRowIndexReader<NID> rowIndexReader;
        private readonly ITableContextReader<NID> tableContextReader;
        private readonly ITableContextBasedPropertyReader<NID> nidBasedTableContextBasedPropertyReader;
        private readonly ITableContextBasedPropertyReader<Tag> tagBasedTableContextBasedPropertyReader;
        private readonly ISubNodesEnumerator subnodesEnumerator;
        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly IPropertyReader propertyReader;        
        private readonly IMapper<NID, LNBTEntry> nidToLNBTEntryMapper;

        internal Folder(
            NID nodeId,
            IDecoder<NID> nidDecoder,
            IRowIndexReader<NID> rowIndexReader,
            ITableContextReader<NID> tableContextReader,
            ITableContextBasedPropertyReader<NID> nidBasedTableContextBasedPropertyReader,
            ITableContextBasedPropertyReader<Tag> tagBasedTableContextBasedPropertyReader,
            ISubNodesEnumerator subnodesEnumerator,
            IPropertyNameToIdMap propertyNameToIdMap,
            IPropertyReader propertyReader,
            IMapper<NID, LNBTEntry> nidToLNBTEntryMapper)
        {
            this.nodeId = nodeId;
            this.nidBasedTableContextBasedPropertyReader = nidBasedTableContextBasedPropertyReader;
            this.nidDecoder = nidDecoder;
            this.rowIndexReader = rowIndexReader;
            this.tableContextReader = tableContextReader;
            this.tagBasedTableContextBasedPropertyReader = tagBasedTableContextBasedPropertyReader;
            this.subnodesEnumerator = subnodesEnumerator;
            this.propertyNameToIdMap = propertyNameToIdMap;
            this.propertyReader = propertyReader;            
            this.nidToLNBTEntryMapper = nidToLNBTEntryMapper;
        }

        public Folder[] GetSubFolders()
        {
            var lnbtEntry =
                nidToLNBTEntryMapper.Map(nodeId.ChangeType(Globals.NID_TYPE_HIERARCHY_TABLE));

            var rowIds =
                rowIndexReader.GetAllRowIds(lnbtEntry.DataBlockId);

            return
                rowIds
                .Select(
                    r =>
                    new Folder(
                        nidDecoder.Decode(r.RowId),
                        nidDecoder,
                        rowIndexReader,
                        tableContextReader, 
                        nidBasedTableContextBasedPropertyReader,                        
                        tagBasedTableContextBasedPropertyReader,
                        subnodesEnumerator,
                        propertyNameToIdMap, 
                        propertyReader,                        
                        nidToLNBTEntryMapper))
                .ToArray();
        }

        public Message[] GetMessages()
        {
            var lnbtEntry =
                nidToLNBTEntryMapper.Map(nodeId.ChangeType(Globals.NID_TYPE_CONTENTS_TABLE));

            var rowIds =
                rowIndexReader.GetAllRowIds(lnbtEntry.DataBlockId);

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
                                rowIndexReader,
                                tableContextReader, 
                                nidBasedTableContextBasedPropertyReader,
                                tagBasedTableContextBasedPropertyReader,
                                subnodesEnumerator,
                                propertyNameToIdMap, 
                                propertyReader);
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
                propertyReader.ReadProperty(
                    lnbtEntry.DataBlockId,
                    lnbtEntry.SubnodeBlockId,
                    propertyTag);
        }
    }
}

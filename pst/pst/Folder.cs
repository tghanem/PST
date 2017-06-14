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

namespace pst
{
    public class Folder
    {
        private readonly NID nodeId;
        private readonly IDecoder<NID> nidDecoder;
        private readonly ITCReader<NID> tcReader;
        private readonly ITCReader<Tag> tagBasedTableContextReader;
        private readonly ISubNodesEnumerator subnodesEnumerator;
        private readonly IPCBasedPropertyReader pcBasedPropertyReader;        
        private readonly IMapper<NID, LNBTEntry> nidToLNBTEntryMapper;

        internal Folder(
            NID nodeId,
            IDecoder<NID> nidDecoder,
            ITCReader<NID> tcReader,
            ITCReader<Tag> tagBasedTableContextReader,
            ISubNodesEnumerator subnodesEnumerator,
            IPCBasedPropertyReader pcBasedPropertyReader,
            IMapper<NID, LNBTEntry> nidToLNBTEntryMapper)
        {
            this.nodeId = nodeId;
            this.tcReader = tcReader;
            this.nidDecoder = nidDecoder;
            this.tagBasedTableContextReader = tagBasedTableContextReader;
            this.subnodesEnumerator = subnodesEnumerator;
            this.pcBasedPropertyReader = pcBasedPropertyReader;            
            this.nidToLNBTEntryMapper = nidToLNBTEntryMapper;
        }

        public Folder[] GetSubFolders()
        {
            var lnbtEntry =
                nidToLNBTEntryMapper.Map(nodeId.ChangeType(Globals.NID_TYPE_HIERARCHY_TABLE));

            var rowIds =
                tcReader.GetAllRowIds(lnbtEntry.DataBlockId);

            return
                rowIds
                .Select(
                    r =>
                    new Folder(
                        nidDecoder.Decode(r.RowId),
                        nidDecoder,
                        tcReader,                        
                        tagBasedTableContextReader,
                        subnodesEnumerator,
                        pcBasedPropertyReader,                        
                        nidToLNBTEntryMapper))
                .ToArray();
        }

        public Message[] GetMessages()
        {
            var lnbtEntry =
                nidToLNBTEntryMapper.Map(nodeId.ChangeType(Globals.NID_TYPE_CONTENTS_TABLE));

            var rowIds =
                tcReader.GetAllRowIds(lnbtEntry.DataBlockId);

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
                                tcReader,
                                tagBasedTableContextReader,
                                subnodesEnumerator,
                                pcBasedPropertyReader);
                    })
                .ToArray();
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

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
        private readonly ITCReader<NID> tcReader;
        private readonly ITCReader<Tag> tagBasedTableContextReader;
        private readonly ISubNodesEnumerator subnodesEnumerator;
        private readonly IPCBasedPropertyReader pcBasedPropertyReader;
        private readonly IDecoder<NID> nidDecoder;
        private readonly IMapper<NID, LNBTEntry> nidToLNBTEntryMapper;

        internal Folder(
            NID nodeId,
            ITCReader<NID> tcReader,
            ITCReader<Tag> tagBasedTableContextReader,
            ISubNodesEnumerator subnodesEnumerator,
            IPCBasedPropertyReader pcBasedPropertyReader,
            IDecoder<NID> nidDecoder,
            IMapper<NID, LNBTEntry> nidToLNBTEntryMapper)
        {
            this.nodeId = nodeId;
            this.tcReader = tcReader;
            this.tagBasedTableContextReader = tagBasedTableContextReader;
            this.subnodesEnumerator = subnodesEnumerator;
            this.pcBasedPropertyReader = pcBasedPropertyReader;
            this.nidDecoder = nidDecoder;
            this.nidToLNBTEntryMapper = nidToLNBTEntryMapper;
        }

        public Folder[] GetSubFolders()
        {
            var lnbtEntry =
                nidToLNBTEntryMapper.Map(nodeId);

            var rowIds =
                tcReader.GetAllRowIds(lnbtEntry.DataBlockId);

            return
                rowIds
                .Select(
                    r =>
                    new Folder(
                        nidDecoder.Decode(r.RowId),
                        tcReader,
                        tagBasedTableContextReader,
                        subnodesEnumerator,
                        pcBasedPropertyReader,
                        nidDecoder,
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
                                messageNodeId,
                                lnbtEntryForMessage.SubnodeBlockId,
                                tcReader,
                                tagBasedTableContextReader,
                                subnodesEnumerator,
                                pcBasedPropertyReader);
                    })
                .ToArray();
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            return pcBasedPropertyReader.ReadProperty(nodeId, propertyTag);
        }
    }
}

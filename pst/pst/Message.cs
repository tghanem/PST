using pst.core;
using pst.encodables;
using pst.encodables.ndb;
using pst.interfaces.ltp.pc;
using pst.interfaces.ltp.tc;
using pst.interfaces.ndb;
using pst.utilities;
using System.Linq;

namespace pst
{
    public class Message
    {
        private readonly NID nodeId;
        private readonly BID subnodeBlockId;
        private readonly ITCReader<NID> nidBasedTableContextReader;
        private readonly ITCReader<Tag> tagBasedTableContextReader;
        private readonly ISubNodesEnumerator subnodesEnumerator;
        private readonly IPCBasedPropertyReader pcBasedPropertyReader;

        internal Message(
            NID nodeId,
            BID subnodeBlockId,
            ITCReader<NID> nidBasedTableContextReader,
            ITCReader<Tag> tagBasedTableContextReader,
            ISubNodesEnumerator subnodesEnumerator,
            IPCBasedPropertyReader pcBasedPropertyReader)
        {
            this.nodeId = nodeId;
            this.subnodeBlockId = subnodeBlockId;

            this.subnodesEnumerator = subnodesEnumerator;
            this.pcBasedPropertyReader = pcBasedPropertyReader;
            this.nidBasedTableContextReader = nidBasedTableContextReader;
            this.tagBasedTableContextReader = tagBasedTableContextReader;
        }

        public Recipient[] Recipients
        {
            get
            {
                var subnodes = 
                    subnodesEnumerator.Enumerate(subnodeBlockId);

                var recipientTableEntry =
                    subnodes.First(s => s.LocalSubnodeId.Type == Globals.NID_TYPE_RECIPIENT_TABLE);

                var rows =
                    nidBasedTableContextReader.GetAllRows(
                        recipientTableEntry.DataBlockId,
                        recipientTableEntry.SubnodeBlockId);

                return
                    rows
                    .Select(
                        r =>
                        {
                            return
                                new Recipient(
                                    recipientTableEntry.DataBlockId,
                                    recipientTableEntry.SubnodeBlockId,
                                    Tag.OfValue(r.RowId),
                                    tagBasedTableContextReader);
                        })
                    .ToArray();
            }
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            return pcBasedPropertyReader.ReadProperty(nodeId, propertyTag);
        }
    }
}

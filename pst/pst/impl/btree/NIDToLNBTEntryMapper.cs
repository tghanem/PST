using pst.core;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.io;

namespace pst.impl.btree
{
    class NIDToLNBTEntryMapper : IMapper<NID, Maybe<LNBTEntry>>
    {
        private readonly IDataReader dataReader;
        private readonly IDecoder<Header> headerDecoder;
        private readonly IBTreeEntryFinder<NID, LNBTEntry, BREF> nodeBTreeEntryFinder;

        public NIDToLNBTEntryMapper(
            IDataReader dataReader,
            IDecoder<Header> headerDecoder,
            IBTreeEntryFinder<NID, LNBTEntry, BREF> nodeBTreeEntryFinder)
        {
            this.dataReader = dataReader;
            this.headerDecoder = headerDecoder;
            this.nodeBTreeEntryFinder = nodeBTreeEntryFinder;
        }

        public Maybe<LNBTEntry> Map(NID input)
        {
            var header = headerDecoder.Decode(dataReader.Read(0, 546));

            return nodeBTreeEntryFinder.Find(input, header.Root.NBTRootPage);
        }
    }
}

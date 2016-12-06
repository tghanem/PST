using pst.core;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.io;

namespace pst.impl.ndb
{
    class BTPageLoader : IBTreeNodeLoader<BTPage, BREF>
    {
        private readonly IDecoder<BTPage> pageDecoder;

        public BTPageLoader(IDecoder<BTPage> pageDecoder)
        {
            this.pageDecoder = pageDecoder;
        }

        public Maybe<BTPage> LoadNode(IDataBlockReader<BREF> reader, BREF pageReference)
        {
            var encodedPage = reader.Read(pageReference, 512);

            return pageDecoder.Decode(encodedPage);
        }
    }
}

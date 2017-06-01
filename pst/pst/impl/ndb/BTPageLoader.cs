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

        private readonly IDataBlockReader<BREF> pageBlockReader;

        public BTPageLoader(
            IDecoder<BTPage> pageDecoder,
            IDataBlockReader<BREF> pageBlockReader)
        {
            this.pageDecoder = pageDecoder;
            this.pageBlockReader = pageBlockReader;
        }

        public BTPage LoadNode(BREF pageReference)
        {
            var encodedPage = pageBlockReader.Read(pageReference, 512);

            return pageDecoder.Decode(encodedPage);
        }
    }
}

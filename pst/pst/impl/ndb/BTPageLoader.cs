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
        private readonly IDataReader dataReader;

        private readonly IDecoder<BTPage> pageDecoder;

        public BTPageLoader(IDataReader dataReader, IDecoder<BTPage> pageDecoder)
        {
            this.dataReader = dataReader;
            this.pageDecoder = pageDecoder;
        }

        public Maybe<BTPage> LoadNode(BREF nodeReference)
        {
            var encodedPage = dataReader.Read(nodeReference.ByteIndex, 512);

            return pageDecoder.Decode(encodedPage);
        }
    }
}

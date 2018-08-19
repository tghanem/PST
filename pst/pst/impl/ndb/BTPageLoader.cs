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
        private readonly ICache<BID, BTPage> pageCache;

        public BTPageLoader(IDataReader dataReader, IDecoder<BTPage> pageDecoder, ICache<BID, BTPage> pageCache)
        {
            this.dataReader = dataReader;
            this.pageDecoder = pageDecoder;
            this.pageCache = pageCache;
        }

        public BTPage LoadNode(BREF pageReference)
        {
            if (pageCache.HasValue(pageReference.BlockId))
            {
                return pageCache.GetValue(pageReference.BlockId);
            }

            var encodedPage = dataReader.Read(pageReference.ByteIndex.Value, 512);

            var decodedPage = pageDecoder.Decode(encodedPage);

            pageCache.Add(pageReference.BlockId, decodedPage);

            return decodedPage;
        }
    }
}

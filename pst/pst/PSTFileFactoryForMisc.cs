using pst.encodables.ndb;
using pst.impl;
using pst.impl.decoders.ndb;
using pst.impl.io;
using pst.interfaces;
using pst.utilities.encodings;
using System.IO;

namespace pst
{
    public partial class PSTFile
    {
        private static IEncoding CreateBlockEncoding(
            Stream dataStream,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new EncodingThatDetectsTypeFromTheHeader(
                    CreateHeaderUsageProvider(dataStream, cachedHeaderHolder),
                    new PermutativeEncoding(),
                    new CyclicEncoding(),
                    new NoEncoding());
        }

        private static IEncoder<string> CreateHeaderBasedStringEncoder(
            Stream dataStream,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new HeaderBasedStringEncoder(
                    CreateHeaderUsageProvider(dataStream, cachedHeaderHolder));
        }

        private static IHeaderUsageProvider CreateHeaderUsageProvider(
            Stream dataStream,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new HeaderUsageProvider(
                    new DataReader(dataStream),
                    CreateHeaderDecoder(),
                    cachedHeaderHolder);
        }

        private static IDecoder<Header> CreateHeaderDecoder()
        {
            return
                new HeaderDecoder(
                    new RootDecoder(
                        new BREFDecoder(
                            new BIDDecoder(),
                            new IBDecoder())));
        }
    }
}

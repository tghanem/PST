using pst.encodables.ndb;
using pst.impl;
using pst.impl.decoders.ndb;
using pst.impl.io;
using pst.interfaces;
using pst.utilities;
using System.IO;

namespace pst
{
    public partial class PSTFile
    {
        private static IBlockDataDeObfuscator CreateBlockDataDeObfuscator(
            Stream dataStream)
        {
            return
                new EncodingThatDetectsTypeFromTheHeader(
                    CreateHeaderReader(dataStream), 
                    new PermutativeEncoding(),
                    new CyclicEncoding(),
                    new NoEncoding());
        }

        private static IEncoder<string> CreateHeaderBasedStringEncoder(
            Stream dataStream)
        {
            return
                new HeaderBasedStringEncoder(
                    CreateHeaderReader(dataStream));
        }

        private static IHeaderReader CreateHeaderReader(Stream dataStream)
        {
            return
                new HeaderReader(
                    new DataReader(dataStream),
                    CreateHeaderDecoder());
        }

        private static IDecoder<Header> CreateHeaderDecoder()
        {
            return
                new HeaderDecoder(
                    new RootDecoder(
                        new BREFDecoder(
                            new BIDDecoder(),
                            new IBDecoder())),
                    new BIDDecoder(),
                    new NIDDecoder());
        }
    }
}

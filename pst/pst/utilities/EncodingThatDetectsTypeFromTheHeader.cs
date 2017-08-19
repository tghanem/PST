using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.io;
using System;

namespace pst.utilities
{
    class EncodingThatDetectsTypeFromTheHeader : IBlockDataObfuscator, IBlockDataDeObfuscator
    {
        private readonly IDataReader dataReader;
        private readonly IDecoder<Header> headerDecoder;

        private readonly IEncoding permutativeEncoding;
        private readonly IEncoding cyclicEncoding;
        private readonly IEncoding noEncoding;

        public EncodingThatDetectsTypeFromTheHeader(IDataReader dataReader, IDecoder<Header> headerDecoder, IEncoding permutativeEncoding, IEncoding cyclicEncoding, IEncoding noEncoding)
        {
            this.dataReader = dataReader;
            this.headerDecoder = headerDecoder;
            this.permutativeEncoding = permutativeEncoding;
            this.cyclicEncoding = cyclicEncoding;
            this.noEncoding = noEncoding;
        }

        public BinaryData Obfuscate(BinaryData blockData, BID blockId)
        {
            var header = GetHeader();

            if (header.CryptMethod == Globals.NDB_CRYPT_NONE)
            {
                return noEncoding.Obfuscate(blockData, blockId);
            }

            if (header.CryptMethod == Globals.NDB_CRYPT_CYCLIC)
            {
                return cyclicEncoding.Obfuscate(blockData, blockId);
            }

            if (header.CryptMethod == Globals.NDB_CRYPT_PERMUTE)
            {
                return permutativeEncoding.Obfuscate(blockData, blockId);
            }

            throw new Exception($"Unexpected bCryptMethod {header.CryptMethod}");
        }

        public BinaryData DeObfuscate(BinaryData blockData, BID blockId)
        {
            var header = GetHeader();

            if (header.CryptMethod == Globals.NDB_CRYPT_NONE)
            {
                return noEncoding.DeObfuscate(blockData, blockId);
            }

            if (header.CryptMethod == Globals.NDB_CRYPT_CYCLIC)
            {
                return cyclicEncoding.DeObfuscate(blockData, blockId);
            }

            if (header.CryptMethod == Globals.NDB_CRYPT_PERMUTE)
            {
                return permutativeEncoding.DeObfuscate(blockData, blockId);
            }

            throw new Exception($"Unexpected bCryptMethod {header.CryptMethod}");
        }

        private Header GetHeader()
        {
            return headerDecoder.Decode(dataReader.Read(0, 546));
        }
    }
}

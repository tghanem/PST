using System;
using pst.encodables.ndb;
using pst.interfaces;

namespace pst.utilities.encodings
{
    class EncodingThatDetectsTypeFromTheHeader : IBlockDataObfuscator, IBlockDataDeObfuscator
    {
        private readonly IHeaderReader headerReader;

        private readonly IEncoding permutativeEncoding;
        private readonly IEncoding cyclicEncoding;
        private readonly IEncoding noEncoding;

        public EncodingThatDetectsTypeFromTheHeader(IHeaderReader headerReader, IEncoding permutativeEncoding, IEncoding cyclicEncoding, IEncoding noEncoding)
        {
            this.headerReader = headerReader;
            this.permutativeEncoding = permutativeEncoding;
            this.cyclicEncoding = cyclicEncoding;
            this.noEncoding = noEncoding;
        }

        public BinaryData Obfuscate(BinaryData blockData, BID blockId)
        {
            var header = headerReader.GetHeader();

            if (header.CryptMethod == Constants.NDB_CRYPT_NONE)
            {
                return noEncoding.Obfuscate(blockData, blockId);
            }

            if (header.CryptMethod == Constants.NDB_CRYPT_CYCLIC)
            {
                return cyclicEncoding.Obfuscate(blockData, blockId);
            }

            if (header.CryptMethod == Constants.NDB_CRYPT_PERMUTE)
            {
                return permutativeEncoding.Obfuscate(blockData, blockId);
            }

            throw new Exception($"Unexpected bCryptMethod {header.CryptMethod}");
        }

        public BinaryData DeObfuscate(BinaryData blockData, BID blockId)
        {
            var header = headerReader.GetHeader();

            if (header.CryptMethod == Constants.NDB_CRYPT_NONE)
            {
                return noEncoding.DeObfuscate(blockData, blockId);
            }

            if (header.CryptMethod == Constants.NDB_CRYPT_CYCLIC)
            {
                return cyclicEncoding.DeObfuscate(blockData, blockId);
            }

            if (header.CryptMethod == Constants.NDB_CRYPT_PERMUTE)
            {
                return permutativeEncoding.DeObfuscate(blockData, blockId);
            }

            throw new Exception($"Unexpected bCryptMethod {header.CryptMethod}");
        }
    }
}

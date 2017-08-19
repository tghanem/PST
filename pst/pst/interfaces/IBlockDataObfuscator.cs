using pst.encodables.ndb;
using pst.utilities;

namespace pst.interfaces
{
    interface IBlockDataObfuscator
    {
        BinaryData Obfuscate(BinaryData blockData, BID blockId);
    }

    interface IBlockDataDeObfuscator
    {
        BinaryData DeObfuscate(BinaryData blockData, BID blockId);
    }

    interface IEncoding : IBlockDataObfuscator, IBlockDataDeObfuscator
    {
    }
}
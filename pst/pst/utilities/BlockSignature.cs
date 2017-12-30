using pst.encodables.ndb;

namespace pst.utilities
{
    static class BlockSignature
    {
        public static int Calculate(IB blockOffset, BID blockId)
        {
            var result = blockOffset.Value ^ blockId.Value;

            return (int)(result >> 16) ^ (int)result;
        }
    }
}

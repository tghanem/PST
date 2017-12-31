using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.encoders.ndb.btree
{
    class PageTrailerEncoder : IEncoder<PageTrailer>
    {
        public BinaryData Encode(PageTrailer value)
        {
            return
                BinaryDataGenerator.New()
                .Append((byte)value.PageType)
                .Append((byte)value.PageTypeRepeat)
                .Append((short)value.PageSignature)
                .Append(value.Crc32ForPageData)
                .Append(value.PageBlockId.Value)
                .GetData();
        }
    }
}

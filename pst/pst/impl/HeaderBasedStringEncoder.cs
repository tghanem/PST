using pst.interfaces;
using pst.utilities;
using System;
using System.Text;

namespace pst.impl
{
    class HeaderBasedStringEncoder : IEncoder<string>
    {
        private readonly IHeaderReader headerReader;

        public HeaderBasedStringEncoder(IHeaderReader headerReader)
        {
            this.headerReader = headerReader;
        }

        public BinaryData Encode(string value)
        {
            var header = headerReader.GetHeader();

            if (header.Version == 14 || header.Version == 15)
            {
                return BinaryData.OfValue(Encoding.Unicode.GetBytes(value));
            }

            if (header.Version == 23)
            {
                return BinaryData.OfValue(Encoding.ASCII.GetBytes(value));
            }

            throw new Exception($"Unsupported PST file encoding {header.Version}");
        }
    }
}

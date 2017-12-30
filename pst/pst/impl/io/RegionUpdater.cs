using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.io;
using pst.utilities;
using System;
using System.IO;

namespace pst.impl.io
{
    class RegionUpdater<TType> : IRegionUpdater<TType>
    {
        private readonly Stream stream;
        private readonly IEncoder<TType> typeEncoder;
        private readonly IDecoder<TType> typeDecoder;
        private readonly int typeSize;

        public RegionUpdater(
            Stream stream,
            IEncoder<TType> typeEncoder,
            IDecoder<TType> typeDecoder,
            int typeSize)
        {
            this.stream = stream;
            this.typeEncoder = typeEncoder;
            this.typeDecoder = typeDecoder;
            this.typeSize = typeSize;
        }

        public void Update(IB regionOffset, Func<TType, TType> processRegion)
        {
            var originalPosition = stream.Position;

            try
            {
                stream.Position = regionOffset;

                var rawRegion = new byte[typeSize];

                stream.Read(rawRegion, 0, typeSize);

                var type = typeDecoder.Decode(BinaryData.OfValue(rawRegion));

                var updatedType = processRegion(type);

                stream.Position = regionOffset;

                var encodedType = typeEncoder.Encode(updatedType);

                stream.Write(encodedType.Value, 0, encodedType.Value.Length);
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }
    }
}

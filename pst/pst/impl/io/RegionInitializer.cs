using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.io;
using System.IO;

namespace pst.impl.io
{
    class RegionInitializer<TType> : IRegionInitializer<TType>
    {
        private readonly Stream dataStream;
        private readonly IEncoder<TType> dataEncoder;

        public RegionInitializer(Stream dataStream, IEncoder<TType> dataEncoder)
        {
            this.dataStream = dataStream;
            this.dataEncoder = dataEncoder;
        }

        public void Initialize(IB regionOffset, TType data)
        {
            var originalPosition = dataStream.Position;

            try
            {
                dataStream.Position = regionOffset;

                var encodedData = dataEncoder.Encode(data);

                dataStream.Write(encodedData, 0, encodedData.Length);
            }
            finally
            {
                dataStream.Position = originalPosition;
            }
        }
    }
}

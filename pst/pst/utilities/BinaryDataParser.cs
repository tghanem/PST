using pst.interfaces;
using System.Collections.Generic;
using System.IO;

namespace pst.utilities
{
    class BinaryDataParser
    {
        private readonly Stream stream;

        private BinaryDataParser(Stream stream)
        {
            this.stream = stream;
        }

        public static BinaryDataParser OfValue(Stream stream)
        {
            return new BinaryDataParser(stream);
        }

        public static BinaryDataParser OfValue(BinaryData data)
        {
            return new BinaryDataParser(new MemoryStream(data.Value));
        }

        public BinaryDataParser Skip(int count)
        {
            stream.Position += count;

            return this;
        }

        public BinaryData[] Slice(int numberOfItems, int itemSize)
        {
            var data = new List<BinaryData>();

            for (int i = 0; i < numberOfItems; i++)
            {
                data.Add(TakeAndSkip(itemSize));
            }

            return data.ToArray();
        }

        public BinaryData TakeAndSkip(int count)
        {
            var buffer = new byte[count];

            stream.Read(buffer, 0, count);

            return BinaryData.OfValue(buffer);
        }

        public TType TakeAndSkip<TType>(int count, IDecoder<TType> typeDecoder)
        {
            var buffer = new byte[count];

            stream.Read(buffer, 0, count);

            return typeDecoder.Decode(BinaryData.OfValue(buffer));
        }

        public TType[] TakeAndSkip<TType>(int numberOfItems, int itemSize, IDecoder<TType> typeDecoder)
        {
            var entries = new List<TType>();

            for (var i = 0; i < numberOfItems; i++)
            {
                entries.Add(TakeAndSkip(itemSize, typeDecoder));
            }

            return entries.ToArray();
        }

        public TType TakeAtWithoutChangingStreamPosition<TType>(int offset, int count, IDecoder<TType> typeDecoder)
        {
            var position = stream.Position;

            stream.Seek(offset, SeekOrigin.Begin);

            var typeValue = TakeAndSkip(count, typeDecoder);

            stream.Position = position;

            return typeValue;
        }
    }
}

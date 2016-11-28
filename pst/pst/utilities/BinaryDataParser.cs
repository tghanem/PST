using pst.interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace pst.utilities
{
    class BinaryDataParser : IDisposable
    {
        private readonly MemoryStream valueStream;

        private BinaryDataParser(MemoryStream valueStream)
        {
            this.valueStream = valueStream;
        }

        public static BinaryDataParser OfValue(BinaryData data)
            => new BinaryDataParser(new MemoryStream(data.Value));

        public BinaryData TakeAt(int offset, int count)
        {
            valueStream.Seek(offset, SeekOrigin.Begin);

            var buffer = new byte[count];

            valueStream.Read(buffer, offset, count);

            return BinaryData.OfValue(buffer);
        }

        public BinaryData TakeAndSkip(int count)
        {
            var buffer = new byte[count];

            valueStream.Read(buffer, 0, count);

            return BinaryData.OfValue(buffer);
        }

        public TType TakeAndSkip<TType>(int count, IDecoder<TType> typeDecoder)
        {
            var buffer = new byte[count];

            valueStream.Read(buffer, 0, count);

            return typeDecoder.Decode(BinaryData.OfValue(buffer));
        }

        public TType[] TakeAndSkip<TType>(int numberOfItems, int itemSize, IDecoder<TType> typeDecoder)
        {
            var entries = new List<TType>();

            for(var i = 0; i < numberOfItems; i++)
            {
                entries
                    .Add(TakeAndSkip(itemSize, typeDecoder));
            }

            return entries.ToArray();
        }

        public TType TakeAtWithoutChangingStreamPosition<TType>(int offset, int count, IDecoder<TType> typeDecoder)
        {
            var position = valueStream.Position;

            valueStream.Seek(offset, SeekOrigin.Begin);

            var typeValue = TakeAndSkip(count, typeDecoder);

            valueStream.Position = position;

            return typeValue;
        }

        public void Dispose()
        {
            valueStream.Dispose();
        }
    }
}

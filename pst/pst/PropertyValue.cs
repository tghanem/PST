using System.Collections.Generic;
using System.Linq;
using pst.utilities;

namespace pst
{
    public class PropertyValue
    {
        public static readonly PropertyValue Empty = new PropertyValue(BinaryData.Empty());

        public BinaryData Value { get; }

        public PropertyValue(BinaryData value)
        {
            Value = value;
        }

        public BinaryData[] GetMultipleValues()
        {
            var count = Value.Take(4).ToInt32();

            var values = new List<BinaryData>();

            var offsets =
                Value
                .Take(4, count * 4)
                .Slice(4)
                .Select(d => d.ToInt32())
                .ToArray();

            for (var i = 0; i < offsets.Length; i++)
            {
                var length = 0;

                if (i < offsets.Length - 1)
                {
                    length = offsets[i + 1] - offsets[i];
                }
                else
                {
                    length = Value.Length - offsets[i];
                }

                values.Add(Value.Take(offsets[i], length));
            }

            return values.ToArray();
        }
    }
}

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
    }
}

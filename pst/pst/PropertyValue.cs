using pst.utilities;

namespace pst
{
    public class PropertyValue
    {
        public BinaryData Value { get; }

        public PropertyValue(BinaryData value)
        {
            Value = value;
        }
    }
}

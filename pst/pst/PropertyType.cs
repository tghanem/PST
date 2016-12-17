namespace pst
{
    public class PropertyType
    {
        public int Value { get; }

        public PropertyType(int value)
        {
            Value = value;
        }

        public static PropertyType OfValue(int value) => new PropertyType(value);
    }
}

namespace pst
{
    public class PropertyId
    {
        public int Value { get; }

        public PropertyId(int value)
        {
            Value = value;
        }

        public static PropertyId OfValue(int value) => new PropertyId(value);

        public override bool Equals(object obj)
        {
            var propertyId = obj as PropertyId;

            return propertyId?.Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}

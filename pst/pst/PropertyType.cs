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

        public override bool Equals(object obj)
        {
            var type = obj as PropertyType;

            return type?.Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return $"0x{Value.ToString("x")}".ToLower();
        }
    }
}

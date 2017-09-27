namespace Machine.Specifications.ReSharperRunner
{
    public struct HashCode
    {
        private readonly int _value;

        private HashCode(int value)
        {
            _value = value;
        }

        public static implicit operator int(HashCode hashCode)
        {
            return hashCode._value;
        }

        public static implicit operator HashCode(int value)
        {
            return new HashCode(value);
        }

        public static HashCode Of<T>(T item)
        {
            return GetHashCode(item);
        }

        public HashCode And<T>(T item)
        {
            return CombineHashCodes(_value, GetHashCode(item));
        }

        private static int CombineHashCodes(int h1, int h2)
        {
            return ((h1 << 5) + h1) ^ h2;
        }

        private static int GetHashCode<T>(T item)
        {
            return item == null ? 0 : item.GetHashCode();
        }
    }
}
namespace Machine.Specifications.Runner.ReSharper.Runner
{
    public readonly struct HashCode
    {
        private readonly int value;

        private HashCode(int value)
        {
            this.value = value;
        }

        public static implicit operator int(HashCode hashCode)
        {
            return hashCode.value;
        }

        public static implicit operator HashCode(int value)
        {
            return new(value);
        }

        public static HashCode Of<T>(T item)
        {
            return GetHashCode(item);
        }

        public HashCode And<T>(T item)
        {
            return CombineHashCodes(value, GetHashCode(item));
        }

        private static int CombineHashCodes(int h1, int h2)
        {
            return ((h1 << 5) + h1) ^ h2;
        }

        private static int GetHashCode<T>(T item)
        {
            return item == null
                ? 0
                : item.GetHashCode();
        }
    }
}

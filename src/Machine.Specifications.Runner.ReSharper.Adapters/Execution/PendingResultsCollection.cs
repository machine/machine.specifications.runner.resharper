using System.Collections.Concurrent;
using System.Collections.Generic;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Execution
{
    public class PendingResultsCollection
    {
        private readonly Dictionary<string, ConcurrentBag<IMspecElement>> values = new();

        public void Add(string key, IMspecElement value)
        {
            if (!values.TryGetValue(key, out var bag))
            {
                values[key] = bag = new ConcurrentBag<IMspecElement>();
            }

            bag.Add(value);
        }

        public IMspecElement? Take(string key)
        {
            if (values.TryGetValue(key, out var bag) && bag.TryTake(out var value))
            {
                return value;
            }

            return null;
        }
    }
}

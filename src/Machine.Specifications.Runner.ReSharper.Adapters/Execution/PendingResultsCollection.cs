using System.Collections.Concurrent;
using System.Collections.Generic;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Execution
{
    public class PendingResultsCollection
    {
        private readonly Dictionary<string, ConcurrentBag<IMspecElement>> values = new();

        public PendingResultsCollection(IMspecElement[] elements)
        {
            foreach (var element in elements)
            {
                if (!values.TryGetValue(element.AggregateId, out var bag))
                {
                    values[element.AggregateId] = bag = new ConcurrentBag<IMspecElement>();
                }

                bag.Add(element);
            }
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

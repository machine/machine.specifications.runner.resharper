using System.Collections.Concurrent;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Execution;

public class ConcurrentLookup<T>
    where T : IMspecElement
{
    private readonly ConcurrentDictionary<string, ConcurrentBag<T>> values = new();

    public void Add(T? element)
    {
        if (element == null)
        {
            return;
        }

        var bag = values.GetOrAdd(element.AggregateId, _ => new ConcurrentBag<T>());

        bag.Add(element);
    }

    public T? Take(string id)
    {
        if (values.TryGetValue(id, out var bag) && bag.TryTake(out var value))
        {
            return value;
        }

        return default;
    }
}

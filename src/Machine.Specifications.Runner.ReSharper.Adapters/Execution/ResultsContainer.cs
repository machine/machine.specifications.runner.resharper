using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Execution
{
    public class ResultsContainer
    {
        private readonly ILookup<string, IMspecElement> elementsById;

        private readonly Dictionary<IMspecElement, ResultsElement> resultsByElement;

        private readonly HashSet<string> behaviorTypes;

        private readonly PendingResultsCollection pending;

        public ResultsContainer(IMspecElement[] elements)
        {
            var contexts = elements
                .OfType<IContextElement>()
                .Select(x =>
                {
                    var specifications = elements
                        .OfType<ISpecificationElement>()
                        .Where(y => y.Context == x && y.Behavior is null)
                        .Select(y => new ResultsElement(y));

                    var behaviors = elements
                        .OfType<IBehaviorElement>()
                        .Where(y => y.Context == x)
                        .Select(y =>
                        {
                            var behaviorSpecifications = elements
                                .OfType<ISpecificationElement>()
                                .Where(z => z.Behavior == y)
                                .Select(z => new ResultsElement(z));

                            return new ResultsElement(y, behaviorSpecifications.ToArray());
                        });

                    return new ResultsElement(x, behaviors.Concat(specifications).ToArray());
                });

            var results = GetResults(contexts.ToArray())
                .ToArray();

            var behaviors = results
                .Select(x => x.Element)
                .OfType<IBehaviorElement>()
                .Select(x => x.TypeName)
                .Distinct();

            pending = new PendingResultsCollection(results.Select(x => x.Element).ToArray());

            elementsById = results
                .Select(x => x.Element)
                .ToLookup(x => x.Id);

            resultsByElement = results.ToDictionary(x => x.Element);

            behaviorTypes = new HashSet<string>(behaviors);
        }

        public T? Started<T>(string id)
            where T : IMspecElement
        {
            var element = pending.Take(id);

            if (element != null && resultsByElement.TryGetValue(element, out var result))
            {
                result.Started();

                return (T) element;
            }

            return default;
        }

        public ResultsElement Get(string id)
        {
            var element = elementsById[id].First();

            return resultsByElement[element];
        }

        public ResultsElement Finished(IMspecElement element, bool isSuccessful)
        {
            var result = resultsByElement[element];

            result.Finished(isSuccessful);

            return result;
        }

        public bool IsBehavior(string type)
        {
            return behaviorTypes.Contains(type);
        }

        private IEnumerable<ResultsElement> GetResults(ResultsElement[] results)
        {
            foreach (var result in results)
            {
                yield return result;

                foreach (var child in GetResults(result.Children.ToArray()))
                {
                    yield return child;
                }
            }
        }
    }
}

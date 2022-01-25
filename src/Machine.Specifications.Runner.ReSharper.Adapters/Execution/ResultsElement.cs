using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Execution
{
    public class ResultsElement
    {
        private Stopwatch? watch;

        private bool? successful;

        public ResultsElement(IMspecElement element, ResultsElement[]? children = null)
        {
            Element = element;
            Children = children ?? Array.Empty<ResultsElement>();
        }

        public IMspecElement Element { get; }

        public IReadOnlyCollection<ResultsElement> Children { get; }

        public TimeSpan Elapsed { get; private set; }

        public bool IsSuccessful => successful ?? Children.Any() && Children.All(x => x.IsSuccessful);

        public void SetResult(bool isSuccessful)
        {
            
        }

        public void Started()
        {
            watch = Stopwatch.StartNew();
        }

        public void Finished(bool isSuccessful)
        {
            if (!Children.Any())
            {
                successful = isSuccessful;
            }

            watch!.Stop();

            Elapsed = watch.Elapsed;
        }
    }
}

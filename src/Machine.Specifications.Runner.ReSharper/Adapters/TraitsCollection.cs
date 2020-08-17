using System.Collections.Generic;
using JetBrains.ReSharper.TestRunner.Abstractions.Serialization;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    [Marshaller(typeof(Marshaller))]
    public class TraitsCollection : Dictionary<string, List<string>>
    {
        private class Marshaller : IUnsafeMarshaller<TraitsCollection>
        {
            public TraitsCollection Read(IUnsafeReader reader)
            {
                throw new System.NotImplementedException();
            }

            public void Write(IUnsafeWriter writer, TraitsCollection value)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}

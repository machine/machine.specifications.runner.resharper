using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public class MspecController
    {
        private readonly CancellationToken token;

        private readonly object controller;

        private readonly MethodInfo invoker;

        public MspecController(CancellationToken token)
        {
            this.token = token;
            var controllerType = Type.GetType("Machine.Specifications.Controller.Controller, Machine.Specifications");

            invoker = controllerType.GetMethod("DiscoverSpecs", BindingFlags.Instance | BindingFlags.Public);
            controller = Activator.CreateInstance(controllerType, (Action<string>) Listener);
        }

        public IEnumerable<TestElement> Find(string assemblyPath)
        {
            var assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
            var assembly = Assembly.Load(assemblyName);

            var results = (string)invoker.Invoke(controller, new object[] { assembly });

            var serializer = new XmlSerializer(typeof(ContextSpecifications));

            using var reader = new StringReader(results);
            using var xmlReader = new XmlTextReader(reader)
            {
                Namespaces = false
            };

            var specifications = (ContextSpecifications)serializer.Deserialize(xmlReader);

            return GetTestElements(specifications);
        }

        private IEnumerable<TestElement> GetTestElements(ContextSpecifications specifications)
        {
            var elements = new List<TestElement>();

            foreach (var context in specifications.Contexts)
            {
                token.ThrowIfCancellationRequested();

                elements.Add(context);

                foreach (var specification in context.Specifications)
                {
                    token.ThrowIfCancellationRequested();

                    specification.Context = context;

                    elements.Add(specification);
                }
            }

            return elements;
        }

        private void Listener(string _)
        {
            token.ThrowIfCancellationRequested();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.ReSharper.Adapters.Models;

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

        public IEnumerable<IMspecElement> Find(string assemblyPath)
        {
            var assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
            var assembly = Assembly.Load(assemblyName);

            var results = (string)invoker.Invoke(controller, new object[] { assembly });

            var contexts = ReadContexts(results);

            return GetTestElements(contexts.ToArray());
        }

        private IEnumerable<Context> ReadContexts(string xml)
        {
            var document = XDocument.Parse(xml);

            foreach (var contextElement in document.Descendants("contextinfo"))
            {
                token.ThrowIfCancellationRequested();

                var context = new Context
                {
                    Name = contextElement.ReadValue("name"),
                    TypeName = contextElement.ReadValue("typename"),
                    Subject = contextElement.ReadValue("concern")
                };

                var specifications = new List<Specification>();

                var specificationsElement = contextElement.Element("specifications");

                if (specificationsElement != null)
                {
                    foreach (var specificationElement in specificationsElement.Elements("specificationinfo"))
                    {
                        token.ThrowIfCancellationRequested();

                        var specification = new Specification
                        {
                            Context = context,
                            Name = specificationElement.ReadValue("name"),
                            ContainingType = specificationElement.ReadValue("containingtype"),
                            FieldName = specificationElement.ReadValue("fieldname")
                        };

                        specifications.Add(specification);
                    }
                }

                context.Specifications = specifications.ToArray();

                yield return context;
            }
        }

        private IEnumerable<IMspecElement> GetTestElements(Context[] contexts)
        {
            foreach (var context in contexts)
            {
                yield return context.AsContext();

                foreach (var specification in context.Specifications)
                {
                    yield return specification.AsSpecification();
                }
            }
        }

        private void Listener(string _)
        {
            token.ThrowIfCancellationRequested();
        }
    }
}

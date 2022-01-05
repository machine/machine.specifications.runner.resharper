using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using Machine.Specifications.Runner.ReSharper.Adapters.Discovery.Elements;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Discovery
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

        public void Find(IMspecDiscoverySink sink, string assemblyPath)
        {
            var assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
            var assembly = Assembly.Load(assemblyName);

            var results = (string)invoker.Invoke(controller, new object[] { assembly });

            var contexts = ReadContexts(results, assembly);

            foreach (var context in contexts)
            {
                sink.OnContext(context);
            }

            sink.OnDiscoveryCompleted();
        }

        private IEnumerable<Context> ReadContexts(string xml, Assembly assembly)
        {
            var document = XDocument.Parse(xml);

            foreach (var contextElement in document.Descendants("contextinfo"))
            {
                token.ThrowIfCancellationRequested();

                var context = new Context
                {
                    Assembly = assembly,
                    Name = contextElement.ReadValue("name"),
                    TypeName = contextElement.ReadValue("typename"),
                    Subject = contextElement.ReadValue("concern")
                };

                var specifications = new List<Specification>();
                var behaviors = new ConcurrentDictionary<string, Specification>();

                var specificationsElement = contextElement.Element("specifications");

                if (specificationsElement != null)
                {
                    foreach (var specificationElement in specificationsElement.Elements("specificationinfo"))
                    {
                        token.ThrowIfCancellationRequested();

                        var specification = new Specification
                        {
                            Assembly = assembly,
                            Context = context,
                            Name = specificationElement.ReadValue("name"),
                            ContainingType = specificationElement.ReadValue("containingtype"),
                            FieldName = specificationElement.ReadValue("fieldname")
                        };

                        SetSpecificationField(assembly, context, specification, behaviors);

                        specifications.Add(specification);
                    }
                }

                context.Specifications = specifications.ToArray();

                yield return context;
            }
        }

        private void SetSpecificationField(Assembly assembly, Context context, Specification specification, ConcurrentDictionary<string, Specification> behaviors)
        {
            var isBehavior = specification.ContainingType != context.TypeName;

            if (isBehavior)
            {
                var fieldName = GetParentFieldName(context, specification);

                var key = $"{context.TypeName}.{fieldName}";

                specification.SpecificationField = behaviors.GetOrAdd(key, _ => CreateParentSpecification(assembly, context, fieldName));
            }
            else
            {
                specification.SpecificationField = specification;
            }
        }

        private Specification CreateParentSpecification(Assembly assembly, Context context, string fieldName)
        {
            return new Specification
            {
                Assembly = assembly,
                Context = context,
                Name = fieldName,
                ContainingType = context.TypeName,
                FieldName = fieldName
            };
        }

        private string? GetParentFieldName(Context context, Specification specification)
        {
            var type = specification.Assembly.GetType(context.TypeName);

            var field = type
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => x.FieldType.IsGenericType)
                .Where(IsBehavesLikeField)
                .Where(x => x.FieldType.GenericTypeArguments.First().FullName == specification.ContainingType)
                .ToArray();

            if (field.Any())
            {
                return field.First().Name;
            }

            return null;
        }

        private bool IsBehavesLikeField(FieldInfo field)
        {
            return field.FieldType.GetCustomAttributes(false)
                .Any(x => x.GetType().FullName == FullNames.BehaviorDelegateAttribute);
        }

        private void Listener(string _)
        {
            token.ThrowIfCancellationRequested();
        }
    }
}

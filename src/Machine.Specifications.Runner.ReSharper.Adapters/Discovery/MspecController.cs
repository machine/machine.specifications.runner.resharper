﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
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

            var results = (string) invoker.Invoke(controller, new object[] {assembly});

            var specifications = GetSpecifications(assembly, results);

            foreach (var specification in specifications)
            {
                sink.OnSpecification(specification);
            }

            sink.OnDiscoveryCompleted();
        }

        private IEnumerable<ISpecificationElement> GetSpecifications(Assembly assembly, string xml)
        {
            var document = XDocument.Parse(xml);

            var cache = new DiscoveryCache();

            foreach (var contextElement in document.Elements("contextinfo"))
            {
                token.ThrowIfCancellationRequested();

                var context = ContextInfo.Parse(contextElement.ToString())
                    .ToElement();

                foreach (var specificationElement in contextElement.Elements("specifications/specificationinfo"))
                {
                    token.ThrowIfCancellationRequested();

                    var specification = SpecificationInfo.Parse(specificationElement.ToString());

                    var behavior = specification.IsBehavior(context.TypeName)
                        ? cache.GetOrAddBehavior(assembly, context, specification)
                        : null;

                    yield return SpecificationInfo.Parse(specificationElement.ToString())
                        .ToElement(context, behavior);
                }
            }
        }

        private void Listener(string _)
        {
            token.ThrowIfCancellationRequested();
        }
    }
}
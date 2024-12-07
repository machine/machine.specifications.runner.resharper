using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Discovery;

public class MspecController : IMspecController
{
    private readonly TestRequest request;

    private readonly CancellationToken token;

    private readonly object controller;

    private readonly MethodInfo invoker;

    public MspecController(TestRequest request, CancellationToken token)
    {
        this.request = request;
        this.token = token;

        var controllerType = GetControllerType();

        invoker = controllerType.GetMethod("DiscoverSpecs", BindingFlags.Instance | BindingFlags.Public)!;
        controller = Activator.CreateInstance(controllerType, (Action<string>) Listener);
    }

    public void Find(IMspecDiscoverySink sink, string assemblyPath)
    {
        var assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
        var assembly = Assembly.Load(assemblyName);

        var results = (string) invoker.Invoke(controller, [assembly]);

        var specifications = GetSpecifications(assembly, results);

        foreach (var specification in specifications)
        {
            sink.OnSpecification(specification);
        }

        sink.OnDiscoveryCompleted();
    }

    private Type GetControllerType()
    {
        var type = Type.GetType("Machine.Specifications.Controller.Controller, Machine.Specifications");

        if (type == null)
        {
            var path = Path.GetDirectoryName(request.Container.Location);
            var assemblyPath = Path.Combine(path!, "Machine.Specifications.dll");

            var assembly = Assembly.LoadFrom(assemblyPath);

            type = assembly.GetType("Machine.Specifications.Controller.Controller");
        }

        if (type == null)
        {
            throw new InvalidOperationException("Cannot find 'Machine.Specifications.dll' controller type");
        }

        return type;
    }

    private IEnumerable<ISpecificationElement> GetSpecifications(Assembly assembly, string xml)
    {
        var document = XDocument.Parse(xml);

        var cache = new DiscoveryCache(assembly);

        foreach (var contextElement in document.Descendants("contextinfo"))
        {
            token.ThrowIfCancellationRequested();

            var contextInfo = ContextInfo.Parse(contextElement.ToString());
            var contextIgnore = cache.GetIgnoreReason(contextInfo.TypeName);

            var context = contextInfo.ToElement(contextIgnore);

            foreach (var specificationElement in contextElement.Descendants("specificationinfo"))
            {
                token.ThrowIfCancellationRequested();

                var specification = SpecificationInfo.Parse(specificationElement.ToString());

                var behavior = specification.IsBehavior(context.TypeName)
                    ? cache.GetOrAddBehavior(context, specification)
                    : null;

                var specificationIgnore = behavior != null
                    ? cache.GetIgnoreReason(behavior.TypeName, specification.FieldName, false) ?? behavior.IgnoreReason
                    : cache.GetIgnoreReason(context.TypeName, specification.FieldName, true);

                yield return SpecificationInfo.Parse(specificationElement.ToString())
                    .ToElement(context, specificationIgnore, behavior);
            }
        }
    }

    private void Listener(string _)
    {
        token.ThrowIfCancellationRequested();
    }
}

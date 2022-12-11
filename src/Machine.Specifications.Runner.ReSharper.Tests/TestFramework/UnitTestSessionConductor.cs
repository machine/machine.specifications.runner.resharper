using System;
using JetBrains.Application.UI.Actions.ActionManager;
using JetBrains.Application.UI.ActionSystem.ActionBar;
using JetBrains.Application.UI.Components;
using JetBrains.Application.UI.Components.Theming;
using JetBrains.Application.UI.ToolWindowManagement;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.UnitTestFramework.Persistence.LevelDb.Marshalling;
using JetBrains.ReSharper.UnitTestFramework.Session;
using JetBrains.ReSharper.UnitTestFramework.Settings;
using JetBrains.ReSharper.UnitTestFramework.UI.Session;
using JetBrains.Util;
using JetBrains.DataFlow;
using JetBrains.IDE.StackTrace;
using JetBrains.ReSharper.UnitTestExplorer.Session.Preview;
using JetBrains.ReSharper.UnitTestExplorer.Session;
using JetBrains.ReSharper.UnitTestFramework.Execution;
using JetBrains.ReSharper.UnitTestFramework.Initialization;
using JetBrains.ReSharper.UnitTestFramework.Persistence.LevelDb.Caches;
using JetBrains.ReSharper.UnitTestFramework.Persistence;
using JetBrains.ReSharper.UnitTestFramework.UI.ViewModels.TreeModel.Nodes;
using System.Collections.Generic;
using JetBrains.Application.Components;

namespace Machine.Specifications.Runner.ReSharper.Tests.TestFramework;

[SolutionComponent]
public class UnitTestSessionConductor : UnitTestSessionConductorBase, IHideImplementation<DefaultNoopUnitTestSessionConductor>
{
    private readonly IEnumerable<IUnitTestSessionManagerPreviewPanelFactory> previewPanelFactories;

    public UnitTestSessionConductor(
        UnitTestingSerializer serializer,
        Lifetime lifetime,
        ISolution solution,
        ITheming theming,
        IUIApplication application,
        IUnitTestingSettings settings,
        IActionManager actionManager,
        IActionBarManager actionBarManager,
        IPersistentIndexManager persistentIndexManager,
        ToolWindowManager toolWindowManager,
        IUnitTestSessionRepository sessionRepository,
        IEnumerable<IUnitTestSessionManagerPreviewPanelFactory> previewPanelFactories,
        ILogger logger)
        : base(serializer, lifetime, solution, theming, application, settings, actionManager, actionBarManager, persistentIndexManager, toolWindowManager, sessionRepository, logger)
    {
        this.previewPanelFactories = previewPanelFactories;
    }

    public override IUnitTestSessionTreeViewModel OpenSession(IUnitTestSession session, bool activate = true)
    {
        EnsureSessionsAreOpened();

        var viewModel = myViewModels.GetOrAdd(session.Id, _ => CreateViewModel(session));

        if (!activate)
        {
            ActiveSession.Value = viewModel;
        }

        return viewModel;
    }

    public override void CloseSession(IUnitTestSession session)
    {
    }

    private IUnitTestSessionTreeViewModel CreateViewModel(IUnitTestSession session)
    {
        UnitTestSessionDescriptor viewModel;

        lock (myLock)
        {
            if (myViewModels.ContainsKey(session.Id))
            {
                throw new InvalidOperationException($"View model for session with id `{session.Id}` already exists");
            }

            var lifetime = Lifetime.Define(myLifetime);

            var viewable = new ViewableCollection<IUnitTestPreviewPanelFactory>(lifetime.Lifetime);
            viewable.AddRange(previewPanelFactories);

            viewModel = new UnitTestSessionDescriptor(
                lifetime.Lifetime,
                mySolution,
                session,
                mySolution.GetComponent<IUnitTestElementRepository>(),
                mySolution.GetComponent<IUnitTestResultManager>(),
                mySolution.GetComponent<IUnitTestViewModelFactory>(),
                viewable,
                mySolution.GetComponent<StackTracePathResolverCache>(),
                mySolution.GetComponent<IByTargetFrameworkCache>(),
                mySolution.GetComponent<IUnitTestingInitializationAwaiter>(),
                GetViewModelState(session.Id));

            PersistSessionWhenModified(lifetime.Lifetime, viewModel);

            myViewModelsLifetimes[session.Id] = lifetime;
        }

        SessionOpened.Fire(viewModel);

        return viewModel;
    }
}

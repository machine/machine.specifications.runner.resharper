namespace Machine.Specifications.ReSharperProvider
{
    using JetBrains.CommonControls;
    using JetBrains.ReSharper.UnitTestFramework;
    using JetBrains.TreeModels;
    using JetBrains.UI.TreeView;

    using Machine.Specifications.ReSharperProvider.Presentation;

    [UnitTestPresenter]
    public class MSpecUnitTestPresenter : IUnitTestPresenter
    {
        Presenter _presenter;

        public MSpecUnitTestPresenter() { this._presenter = new Presenter(); }

        public void Present(IUnitTestElement element, IPresentableItem item, TreeModelNode node, PresentationState state)
        {
            if (element is Element)
            {
                this._presenter.UpdateItem(element, node, item, state);
            }
        }
    }
}
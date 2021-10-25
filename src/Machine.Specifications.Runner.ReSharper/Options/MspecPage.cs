using JetBrains.Application.Environment;
using JetBrains.Application.Environment.Helpers;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.UnitTestFramework.UI.Options;
using Machine.Specifications.Runner.ReSharper.Resources;

namespace Machine.Specifications.Runner.ReSharper.Options
{
    [OptionsPage("MSpecPage", "MSpec", typeof(MspecThemedIcons.Mspec), ParentId = UnitTestingPages.Frameworks)]
    public class MspecPage : CustomSimpleOptionsPage
    {
        public MspecPage(
            Lifetime lifetime,
            OptionsSettingsSmartContext optionsSettingsSmartContext,
            MspecTestProvider provider,
            RunsProducts.ProductConfigurations productConfigurations)
            : base(lifetime, optionsSettingsSmartContext)
        {
            var supportEnabledOption = UnitTestProviderOptionsPage.GetTestSupportEnabledOption(lifetime, optionsSettingsSmartContext, provider.ID);

            if (productConfigurations.IsInternalMode())
            {
                AddBoolOption(supportEnabledOption, "_Enable " + provider.Name + " support", string.Empty);
            }

            AddHeader("Test discovery");

            this.AddExplorationMethodSelect<MspecProviderSettings>(x => x.TestDiscoveryFromArtifactsMethod);
        }
    }
}

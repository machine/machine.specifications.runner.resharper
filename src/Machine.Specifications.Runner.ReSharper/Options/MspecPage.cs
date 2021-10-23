using JetBrains.Application.Environment;
using JetBrains.Application.Environment.Helpers;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.Application.UI.Options.OptionsDialog.SimpleOptions.ViewModel;
using JetBrains.IDE.UI.Options;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.UnitTestFramework.Exploration.Artifacts;
using JetBrains.ReSharper.UnitTestFramework.UI.Options;
using Machine.Specifications.Runner.ReSharper.Resources;

namespace Machine.Specifications.Runner.ReSharper.Options
{
    [OptionsPage("MSpec", "MSpec", typeof(MspecThemedIcons.MSpec), HelpKeyword = "Reference_Options_Tools_Unit_Testing_MSpec", ParentId = "UnitTesting_Frameworks", Sequence = 2)]
    public class MspecPage : BeSimpleOptionsPage
    {
        public MspecPage(
            Lifetime lifetime,
            OptionsSettingsSmartContext optionsSettingsSmartContext,
            MspecTestProvider provider,
            OptionsPageContext optionsPageContext,
            RunsProducts.ProductConfigurations productConfigurations)
            : base(lifetime, optionsPageContext, optionsSettingsSmartContext)
        {
            var supportEnabledOption = UnitTestProviderOptionsPage.GetTestSupportEnabledOption(lifetime, optionsSettingsSmartContext, provider.ID);

            if (productConfigurations.IsInternalMode())
            {
                AddBoolOption(supportEnabledOption, "_Enable" + provider.Name + " support", string.Empty);
            }

            AddHeader("Test discovery");

            AddRadioOption<MspecProviderSettings, DiscoveryMethod>(
                x => x.TestDiscoveryFromArtifactsMethod,
                "When running tests discovery from artifacts, use:",
                new RadioOptionPoint(DiscoveryMethod.Metadata, "Metadata", "Fast, but might not be able to find certain entities, such as custom categories or dynamic tests. They will appear after the first execution."),
                new RadioOptionPoint(DiscoveryMethod.TestRunner, "Test Runner", "Slower, finds all tests or categories"));
        }
    }
}

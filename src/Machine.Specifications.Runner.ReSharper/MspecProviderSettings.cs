using JetBrains.Application;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Extentions;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.UnitTestFramework.Exploration.Artifacts;
using JetBrains.ReSharper.UnitTestFramework.Settings;

namespace Machine.Specifications.Runner.ReSharper;

[SettingsKey(typeof(UnitTestingSettings), "Settings for MSpec unit test provider")]
public class MspecProviderSettings
{
    [SettingsEntry(DiscoveryMethod.Metadata, "Discovery method for discovery tests from artifacts")]
    public DiscoveryMethod TestDiscoveryFromArtifactsMethod;

    [ShellComponent]
    public class Reader(ISettingsStore settingsStore, ISettingsOptimization settingsOptimization)
        : ICachedSettingsReader<MspecProviderSettings>
    {
        public ISettingsOptimization SettingsOptimization { get; } = settingsOptimization;

        public SettingsKey KeyExposed { get; } = settingsStore.Schema.GetKey<MspecProviderSettings>();

        public MspecProviderSettings ReadData(Lifetime lifetime, IContextBoundSettingsStore store)
        {
            return (MspecProviderSettings)store.GetKey(KeyExposed, null, SettingsOptimization)!;
        }
    }
}

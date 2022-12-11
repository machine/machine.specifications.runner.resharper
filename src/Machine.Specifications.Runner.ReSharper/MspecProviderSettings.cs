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
    public class Reader : ICachedSettingsReader<MspecProviderSettings>
    {
        public Reader(ISettingsStore settingsStore, ISettingsOptimization settingsOptimization)
        {
            SettingsOptimization = settingsOptimization;
            KeyExposed = settingsStore.Schema.GetKey<MspecProviderSettings>();
        }

        public ISettingsOptimization SettingsOptimization { get; }

        public SettingsKey KeyExposed { get; }

        public MspecProviderSettings ReadData(Lifetime lifetime, IContextBoundSettingsStore store)
        {
            return (MspecProviderSettings)store.GetKey(KeyExposed, null, SettingsOptimization)!;
        }
    }
}

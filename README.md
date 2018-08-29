# Machine.Specifications Runner for Resharper

[![Build status](https://ci.appveyor.com/api/projects/status/obdkaoex68nqsubm/branch/master?svg=true)](https://ci.appveyor.com/project/machine-specifications/machine-specifications-runner-resharper/branch/master)

MSpec provides a Resharper plugin integrate with the ReSharper test runner, custom naming rules, and code annotations. MSpec currently supports ReSharper (8.2 and upwards) over the extension gallery. Just search for `Machine.Specifications.Runner.Resharper`.

### Debugging

1) Run the `install-plugin.ps` script the first time to install ReSharper to an experimental hive and deploy the plugin
2) Set the startup project to `Machine.Specifications.ReSharperRunner.Debug.VS`
3) Run and debug from Visual Studio

### Releasing

Releases follow [Semver](https://semver.org) format and are performed via Github's [releases](https://github.com/machine/machine.specifications.runner.resharper/releases) page.
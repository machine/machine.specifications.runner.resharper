# Machine.Specifications Runner for Resharper

[![Build status](https://ci.appveyor.com/api/projects/status/obdkaoex68nqsubm/branch/master?svg=true)](https://ci.appveyor.com/project/machine-specifications/machine-specifications-runner-resharper/branch/master)

MSpec provides a Resharper plugin integrate with the ReSharper test runner, custom naming rules, and code annotations. MSpec currently supports ReSharper (2017.2.2 to 8.2) over the extension gallery. Just search for `Machine.Specifications.Runner.Resharper`.

### Debugging

1) Follow the instructions for deploying a ReSharper package locally [here](https://www.jetbrains.com/help/resharper/sdk/Extensions/Deployment/LocalInstallation.html)
2) Set the startup project to `Machine.Specifications.ReSharperRunner.Debug.VS`
3) Build the project and take note of the warnings `"No installed product MyProductName"`
4) Modify the provider and runner `.csproj` files to replace `MyProductName` with the product name that matches your experimental ReSharper instance (eg. `ReSharperPlatformVs15_abc123mspec`).
5) Run and debug from Visual Studio
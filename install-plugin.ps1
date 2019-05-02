Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent

$PluginId = "Machine.Specifications.Runner.Resharper9"
$RootSuffix = "ReSharper"
$ResharperUrl = "https://data.services.jetbrains.com/products/releases?code=RSU&type=release"
$NugetUrl = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
$Version = "0.1.0"
$MainProject = "$PSScriptRoot\src\Machine.Specifications.Runner.Resharper.Provider\Machine.Specifications.Runner.Resharper.Provider.csproj"
$NugetExe = "$PSScriptRoot\tools\nuget.exe"

Function Invoke-Exe {
    param(
        [Parameter(Mandatory = $true, Position = 0)]
        [ValidateNotNullOrEmpty()]
        [String]
        $Executable,

        [Parameter(ValueFromRemainingArguments = $true)]
        [String[]]
        $Arguments
    )

    Write-Host "> $Executable $Arguments"
    $result = Start-Process -FilePath $Executable -ArgumentList $Arguments -NoNewWindow -Wait -Passthru

    if (-Not $result.ExitCode -eq 0) {
        throw "'$Executable $Arguments' failed with exit code $($result.ExitCode)"
    }
}

Function Write-User-Settings {
    param(
        [Parameter(Mandatory = $true)]
        [String]
        $HostIdentifier,

        [Parameter(Mandatory = $true)]
        [String]
        $UserProjectFile
    )

    if (!(Test-Path "$UserProjectFile")) {
        Set-Content -Path "$UserProjectFile" -Value "<Project><PropertyGroup Condition=`"'`$(Configuration)' == 'Debug'`"><HostFullIdentifier></HostFullIdentifier></PropertyGroup></Project>"
    }

    $xml = [xml] (Get-Content "$UserProjectFile")
    $node = $xml.SelectSingleNode(".//HostFullIdentifier")
    $node.InnerText = $HostIdentifier
    $xml.Save("$UserProjectFile")
}

Function Read-SdkVersion {
    $xml = [xml] (Get-Content "$MainProject")
    $node = $xml.SelectSingleNode(".//SdkVersion")

    if ($null -eq $node) {
        throw "SdkVersion not found in '$MainProject'"
    }

    $value = $node.InnerText

    return $value.Substring(2, 2) + $value.Substring(5, 1)
}

Function Read-PackageVersion {

}

Function Write-Nuspec {
    param(
        [Parameter(Mandatory=$true)]
        [String]
        $NuspecFile
    )

    $sdkVersion = Read-SdkVersion
    $nextSdkVersion = [int]$sdkVersion + 1

    $nuspec = @'
<?xml version="1.0" encoding="utf-8"?>
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
  <metadata>
    <id>Machine.Specifications.Runner.Resharper9</id>
    <version>{0}</version>
    <authors>Machine Specifications</authors>
    <description>Machine Specifications</description>
    <dependencies>
      <dependency id="Wave" version="[{1}.0, {2})" />
    </dependencies>
  </metadata>

  <files>
    <file src="..\src\Machine.Specifications.Runner.Resharper.Provider\bin\Debug\net461\Machine.Specifications.ReSharper.Runner.dll" target="DotFiles" />
    <file src="..\src\Machine.Specifications.Runner.Resharper.Provider\bin\Debug\net461\Machine.Specifications.ReSharper.Provider.dll" target="DotFiles" />
    <file src="..\src\Machine.Specifications.Runner.Resharper.Provider\bin\Debug\net461\Machine.Specifications.Runner.Utility.dll" target="DotFiles" />
  </files>
</package>
'@ -f $Version,$sdkVersion,$nextSdkVersion

    Set-Content -Path "$NuspecFile" -Value $nuspec
}

Function Get-ToolPath {
    param(
        [Parameter(Mandatory = $true)]
        [String]
        $ToolUrl
    )

    $file = $ToolUrl.Substring($ToolUrl.LastIndexOf("/") + 1)

    return "$PSScriptRoot\tools\" + $file
}

Function Get-Tool {
    param(
        [Parameter(Mandatory = $true)]
        [String]
        $ToolUrl
    )

    $file = Get-ToolPath $ToolUrl

    if (!(Test-Path $file)) {
        mkdir -Force $(Split-Path $file -Parent) > $null
        Write-Output "Downloading from $ToolUrl"
        (New-Object System.Net.WebClient).DownloadFile($ToolUrl, $file)
    } else {
        $filename = $file.Substring($file.LastIndexOf("\") + 1)
        Write-Output "Using $($filename) from cache"
    }
}

Function Install-Hive {
    param(
        [Parameter(Mandatory = $true)]
        [String]
        $InstallerFile
    )

    Write-Output "Installing experimental hive"
    Invoke-Exe $InstallerFile "/VsVersion=15.0;16.0" "/SpecificProductNames=ReSharper" "/Hive=$RootSuffix" "/Silent=True"
}

Function Get-InstallationPath {
    $version = Read-SdkVersion

    return $(Get-ChildItem "$env:APPDATA\JetBrains\ReSharperPlatformVs*\v$version`_*$RootSuffix\NuGet.Config" | Sort-Object | Select-Object -Last 1).Directory
}

Function Save-PackagesConfig {
    $installPath = Get-InstallationPath

    if (Test-Path "$installPath\packages.config") {
        $xml = [xml] (Get-Content "$installPath\packages.config")
    } else {
        $xml = [xml] ("<?xml version=`"1.0`" encoding=`"utf-8`"?><packages></packages>")
    }
    
    if ($null -eq $xml.SelectSingleNode(".//package[@id='$PluginId']/@id")) {
        $pluginNode = $xml.CreateElement('package')
        $pluginNode.setAttribute("id", "$PluginId")
        $pluginNode.setAttribute("version", "$Version")
    
        $packagesNode = $xml.SelectSingleNode("//packages")
        $packagesNode.AppendChild($PluginNode) > $null
    
        $xml.Save("$installPath\packages.config")
    }
}

# Download tools
$url = [uri] $(Invoke-WebRequest -UseBasicParsing $ResharperUrl | ConvertFrom-Json).RSU[0].downloads.windows.link
$resharperTool = Get-ToolPath $url

Get-Tool $url
Get-Tool $NugetUrl

# Build plugin
$artifacts = "$PSScriptRoot\artifacts"

Write-Nuspec "$artifacts\Package.nuspec"
Invoke-Exe dotnet build "$PSScriptRoot\Machine.Specifications.Runner.Resharper.sln" /p:Version=$Version /p:HostFullIdentifier="" /p:UseSharedCompilation=false
Invoke-Exe $NugetExe pack "$artifacts\Package.nuspec" -version $Version -outputDirectory "$artifacts"

# Install hive
Install-Hive $resharperTool

# Set up environment
Save-PackagesConfig

# Install plugin
Invoke-Exe $NugetExe install $PluginId -OutputDirectory "$env:LOCALAPPDATA\JetBrains\plugins" -Source "$artifacts" -DependencyVersion Ignore

# Reinstall hive
Install-Hive $resharperTool

# Set user project settings
$installPath = Get-InstallationPath
$hostIdentifier = "$($installPath.Parent.Name)_$($installPath.Name.Split('_')[-1])"

Write-User-Settings $hostIdentifier "$PSScriptRoot\src\Machine.Specifications.Runner.Resharper.Provider\Machine.Specifications.Runner.Resharper.Provider.csproj.user"
Write-User-Settings $hostIdentifier "$PSScriptRoot\src\Machine.Specifications.Runner.Resharper.Runner\Machine.Specifications.Runner.Resharper.Runner.csproj.user"

Write-Output "Installed plugin to hive"

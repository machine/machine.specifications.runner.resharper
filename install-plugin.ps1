Param(
    [switch]
    $UseEap
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Function Invoke-Exe {
    param(
        [parameter(mandatory=$true,position=0)] [ValidateNotNullOrEmpty()] [string] $Executable,
        [Parameter(ValueFromRemainingArguments=$true)][String[]] $Arguments,
        [parameter(mandatory=$false)] [array] $ValidExitCodes = @(0)
    )

    Write-Host "> $Executable $Arguments"
    $rc = Start-Process -FilePath $Executable -ArgumentList $Arguments -NoNewWindow -Wait -Passthru
    if (-Not $ValidExitCodes.Contains($rc.ExitCode)) {
        throw "'$Executable $Arguments' failed with exit code $($rc.ExitCode), valid exit codes: $ValidExitCodes"
    }
}

Function Write-User-Proj {
    param(
        [parameter(mandatory=$true)] [string] $HostIdentifier,
        [parameter(mandatory=$true)] [string] $UserProjectXmlFile
    )

    if (!(Test-Path "$UserProjectXmlFile")) {
        Set-Content -Path "$UserProjectXmlFile" -Value "<Project><PropertyGroup><HostFullIdentifier></HostFullIdentifier></PropertyGroup></Project>"
    }

    $ProjectXml = [xml] (Get-Content "$UserProjectXmlFile")
    $HostIdentifierNode = $ProjectXml.SelectSingleNode(".//HostFullIdentifier")
    $HostIdentifierNode.InnerText = $HostIdentifier
    $ProjectXml.Save("$UserProjectXmlFile")
}

$PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent
$PluginId = "Machine.Specifications.Runner.Resharper9"
$PluginPrefix = "Machine.Specifications.ReSharper"
$RootSuffix = "ReSharper"
$Version = "1.0.0"

# Determine download link
if ($UseEap) {
    $ReleaseUrl = "https://data.services.jetbrains.com/products/releases?code=RSU&type=eap"
} else {
    $ReleaseUrl = "https://data.services.jetbrains.com/products/releases?code=RSU&type=release"
}
$DownloadLink = [uri] $(Invoke-WebRequest -UseBasicParsing $ReleaseUrl | ConvertFrom-Json).RSU[0].downloads.windows.link

# Download installer
$InstallerFile = "$PSScriptRoot\tools\$($DownloadLink.Segments[-1])"
if (!(Test-Path $InstallerFile)) {
    mkdir -Force $(Split-Path $InstallerFile -Parent) > $null
    Write-Output "Downloading from $DownloadLink"
    (New-Object System.Net.WebClient).DownloadFile($DownloadLink, $InstallerFile)
} else {
    Write-Output "Using $($DownloadLink.segments[-1]) from cache"
}

# Download nuget
$NugetDownloadLink = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"

$NugetFile = "$PSScriptRoot\tools\nuget.exe"
if (!(Test-Path $NugetFile)) {
    Write-Output "Downloading from $NugetDownloadLink"
    (New-Object System.Net.WebClient).DownloadFile($NugetDownloadLink, $NugetFile)
} else {
    Write-Output "Using nuget.exe from cache"
}

# Execute installer
Write-Output "Installing experimental hive"
Invoke-Exe $InstallerFile "/VsVersion=15.0" "/SpecificProductNames=ReSharper" "/Hive=$RootSuffix" "/Silent=True"

$PluginRepository = "$env:LOCALAPPDATA\JetBrains\plugins"
$InstallationDirectory = $(Get-ChildItem "$env:APPDATA\JetBrains\ReSharperPlatformVs*\v*_*$RootSuffix\NuGet.Config").Directory

# Adapt packages.config
if (Test-Path "$InstallationDirectory\packages.config") {
    $PackagesXml = [xml] (Get-Content "$InstallationDirectory\packages.config")
} else {
    $PackagesXml = [xml] ("<?xml version=`"1.0`" encoding=`"utf-8`"?><packages></packages>")
}

if ($null -eq $PackagesXml.SelectSingleNode(".//package[@id='$PluginId']/@id")) {
    $PluginNode = $PackagesXml.CreateElement('package')
    $PluginNode.setAttribute("id", "$PluginId")
    $PluginNode.setAttribute("version", "$Version")

    $PackagesNode = $PackagesXml.SelectSingleNode("//packages")
    $PackagesNode.AppendChild($PluginNode) > $null

    $PackagesXml.Save("$InstallationDirectory\packages.config")
}

# Install plugin
$OutputDirectory = "$PSScriptRoot\Build"

Invoke-Exe dotnet build "$PSScriptRoot\Machine.Specifications.Runner.Resharper.sln" /p:Version=$Version /p:HostFullIdentifier=""
Invoke-Exe "$NugetFile" pack "$PSScriptRoot\Machine.Specifications.Runner.Resharper.nuspec" -version $Version -outputDirectory "$OutputDirectory"
Invoke-Exe "$NugetFile" install $PluginId -OutputDirectory "$PluginRepository" -Source "$OutputDirectory" -DependencyVersion Ignore

Write-Output "Re-installing experimental hive"
Invoke-Exe "$InstallerFile" "/VsVersion=15.0" "/SpecificProductNames=ReSharper" "/Hive=$RootSuffix" "/Silent=True"

# Adapt user project file
$HostIdentifier = "$($InstallationDirectory.Parent.Name)_$($InstallationDirectory.Name.Split('_')[-1])"

Write-User-Proj $HostIdentifier "$PSScriptRoot\$PluginPrefix.Provider.10\$PluginPrefix.Provider.10.csproj.user"
Write-User-Proj $HostIdentifier "$PSScriptRoot\$PluginPrefix.Runner.10\$PluginPrefix.Runner.10.csproj.user"

Write-Output "Installed $PluginId to hive $HostIdentifier"
Write-Output "  Set the startup project to 'Machine.Specifications.ReSharperRunner.Debug.VS'"
Write-Output "  and ensure that the launch parameters specify:"
Write-Output "  devenv.exe /rootsuffix $RootSuffix /ReSharper.Internal"

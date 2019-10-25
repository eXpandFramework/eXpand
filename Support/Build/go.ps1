param(
    [string]$configuration = "Release",
    [string]$version = $null,
    [string]$msbuild = $null,
    [string[]]$packageSources = @("https://api.nuget.org/v3/index.json", "https://xpandnugetserver.azurewebsites.net/nuget", "C:\Program Files (x86)\DevExpress 19.1\Components\System\Components\packages")  ,
    [string[]]$msbuildArgs = @("/p:Configuration=$configuration", "/WarnAsError", "/v:m"),
    [string[]]$taskList = @("Release"),
    [string]$nugetApiKey = $null,
    [string]$Repository = "eXpand",
    [string]$XpandPwshVersion = "0.23.3",
    [bool]$ResolveNugetDependecies
)

$XpandPwsh = [PSCustomObject]@{
    Name    = "XpandPwsh"
    Version = $XpandPwshVersion
}

& "$PSScriptRoot\Install-Module.ps1" -psObj $XpandPwsh
$psake = [PSCustomObject]@{
    Name    = "psake"
    Version = "4.7.4"
}
& "$PSScriptRoot\Install-Module.ps1" -psObj $psake
if (!$version) {
    $officialPackages=Get-XpandPackages -PackageType eXpand -Source Release
    $labPackages=Get-XpandPackages -PackageType eXpand -Source Lab
    $DXVersion=Get-DevExpressVersion
    $nextVersion = Get-XXpandVersion -Next -OfficialPackages $officialPackages -LabPackages $labPackages -DXVersion $DXVersion
    Write-host "NextVersion=$nextVersion"
    return $nextVersion
}

if (!$msbuild) {
    $msbuild = Get-XMsBuildPath
}

$clean = $($taskList -in "Release", "lab")
Invoke-Xpsake  "$PSScriptRoot\Build.ps1" -properties @{
    "version"                 = $version;
    "msbuild"                 = $msbuild;
    "clean"                   = $clean;
    "msbuildArgs"             = $msbuildArgs;
    "throttle"                = $throttle;
    "packageSources"          = $packageSources;
    "nugetApiKey"             = $nugetApiKey;
    "Repository"              = $Repository;
    "Release"                 = ($Repository -eq "eXpand")
    "ResolveNugetDependecies" = $ResolveNugetDependecies
} -taskList $taskList

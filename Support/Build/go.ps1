param(
    [string]$configuration = "Release",
    [string]$version = $null,
    [string]$msbuild = $null,
    [string[]]$packageSources = @("https://api.nuget.org/v3/index.json", "https://xpandnugetserver.azurewebsites.net/nuget", "C:\Program Files (x86)\DevExpress 19.1\Components\System\Components\packages")  ,
    [string[]]$msbuildArgs = @("/p:Configuration=$configuration;SkipNugetReplace=true", "/WarnAsError", "/v:m"),
    [string[]]$taskList = @("Release"),
    [string]$nugetApiKey = $null,
    [string]$Repository = "eXpand",
    [string]$XpandPwshVersion = "1.221.0.4",
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
    $ps1=Get-Content "$PSScriptRoot\..\..\build.ps1" -Raw
    $regex = [regex] '\d{2}\.\d{1,2}\.\d{3}.\d{1}'
    [version]$nextVersion=$regex.Match($ps1).Value
    $officialPackages=Get-XpandPackages -PackageType eXpand -Source Release
    $labPackages=Get-XpandPackages -PackageType eXpand -Source Lab
    if ($labPackages){
        $nextVersion = Get-XXpandVersion -Next -OfficialPackages $officialPackages -LabPackages $labPackages -DXVersion $nextVersion
    }
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

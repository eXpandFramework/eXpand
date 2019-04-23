param(
    [string]$configuration="Release",
    [string]$version=$null,
    [string]$msbuild=$null,
    [string[]]$packageSources=@("https://api.nuget.org/v3/index.json","https://xpandnugetserver.azurewebsites.net/nuget","C:\Program Files (x86)\DevExpress 19.1\Components\System\Components\packages")  ,
    [string[]]$msbuildArgs=@("/p:Configuration=$configuration","/WarnAsError","/v:m"),
    [string[]]$taskList=@("Release"),
    [string]$nugetApiKey=$null,
    [switch]$UseAllPackageSources,
    [string]$Repository="eXpand",
    [string]$branch="master"
)
$xpandPosh=[PSCustomObject]@{
    Name = "XpandPosh"
    Version = "1.9.2"
}
& "$PSScriptRoot\Install-Module.ps1" -psObj $xpandPosh
$psake=[PSCustomObject]@{
    Name = "psake"
    Version = "4.7.4"
}
& "$PSScriptRoot\Install-Module.ps1" -psObj $psake
if (!$version){
    $nextVersion=Get-XXpandVersion -Next
    Write-host "NextVersion=$nextVersion"
    return $nextVersion
}

if (!$msbuild){
    $msbuild=Get-XMsBuildPath
}

$clean=$($taskList -in "Release","lab")
Invoke-Xpsake  "$PSScriptRoot\Build.ps1" -properties @{
    "version"=$version;
    "msbuild"=$msbuild;
    "clean"=$clean;
    "msbuildArgs"=$msbuildArgs;
    "throttle"=$throttle;
    "packageSources"=$packageSources;
    "nugetApiKey"=$nugetApiKey;
    "Repository"=$Repository;
    "UseAllPackageSources"=$UseAllPackageSources
    "branch"=$branch
} -taskList $taskList

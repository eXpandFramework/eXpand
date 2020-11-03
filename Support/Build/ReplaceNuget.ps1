using namespace system.text.RegularExpressions
param(
    $ProjectPath = "C:\Work\eXpandFramework\expand\Xpand\Xpand.ExpressApp.Modules\AdditionalViewControlsProvider\Xpand.ExpressApp.AdditionalViewControlsProvider.csproj",
    $TargetPath = "C:\Work\eXpandFramework\expand\Xpand.dll\Xpand.ExpressApp.AdditionalViewControlsProvider.dll",
    $SkipNugetReplace
)

 

$ErrorActionPreference = "Stop"
$eXpandLib="Xpand.Xpo","Xpand.Utils","Xpand.Persistent.Base","Xpand.Persistent.BaseImpl"
$nugetFolder = "$env:USERPROFILE\.nuget\packages"    
if ((Test-Path $nugetFolder) -and !$SkipNugetReplace) {
    $packageId=(Get-Item $ProjectPath).BaseName
    if ($packageId -in $eXpandLib){
        $packageId="eXpandLib"
    }
    else{
        $packageId=$packageId.Replace("Xpand.ExpressApp.","").Replace(".","")
        $packageId="eXpand$packageId"
    }
    $packageFolder = Get-ChildItem $nugetFolder $packageId
    if ($packageFolder){
        $assemblyName = (Get-Item $ProjectPath).BaseName
        Get-ChildItem $packageFolder.FullName "$assemblyName.dll" -Recurse | ForEach-Object {
            Copy-Item $TargetPath $_.FullName -Force -Verbose
        }
    }
}
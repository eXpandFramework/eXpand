using namespace system.text.RegularExpressions
param(
    $ProjectPath = "C:\Work\expand\Xpand\Xpand.ExpressApp.Modules\WizardUI.Win\Xpand.ExpressApp.WizardUI.Win.csproj",
    $TargetPath = "C:\Work\expand\Xpand.DLL\net461\Xpand.ExpressApp.WizardUI.Win.dll",
    $SkipNugetReplace
)
if (!($TargetPath)){
    return
}
 

$ErrorActionPreference = "Stop"

$eXpandLib="Xpand.Xpo","Xpand.Utils","Xpand.Persistent.Base","Xpand.Persistent.BaseImpl"
$system="Xpand.ExpressApp","Xpand.ExpressApp.Win","Xpand.ExpressApp.Web"
$nugetFolder = "$env:USERPROFILE\.nuget\packages"    
if ((Test-Path $nugetFolder) -and !$SkipNugetReplace) {
    $packageId=(Get-Item $ProjectPath).BaseName
    if ($packageId -in $eXpandLib){
        $packageId="eXpandLib"
    }
    else{
        if ($packageId -in $system){
            $system="System"
        }
        else{
            $system=$null
        }
        $packageId=$packageId.Replace("Xpand.ExpressApp.","").Replace(".","")
        $packageId="eXpand$($system)$packageId"
    }
    $packageFolder = Get-ChildItem $nugetFolder $packageId
    $targetItem=Get-Item $targetPath
    if ($packageFolder){
        $assemblyName = (Get-Item $ProjectPath).BaseName
        Get-ChildItem $packageFolder.FullName "$assemblyName.dll" -Recurse | ForEach-Object {
            [PSCustomObject]@{
                Item = $_
                Version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($_.FullName).fileversion
                BaseName=$_.BaseName
            }
        }|Group-Object BaseName|ForEach-Object{
            $item=($_.group|Sort-Object Version -Descending|Select-Object -First 1).Item
            if ($FixVersion){
                # $destinationVersion=$item.Directory.Parent.Parent.Name
                # Update-AssemblyInfoVersion $destinationVersion  $projectItem.DirectoryName
            }
            else{
                $destination=$item.FullName
                Copy-Item $TargetPath $destination -Force -Verbose
                Copy-Item "$($targetItem.DirectoryName)\$($targetItem.BaseName).pdb" $item.DirectoryName -Force -Verbose
            }   
        }
    }
}
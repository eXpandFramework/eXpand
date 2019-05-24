param(
    [string[]]$packageSources=@("https://api.nuget.org/v3/index.json","https://xpandnugetserver.azurewebsites.net/nuget","C:\Program Files (x86)\DevExpress 18.2\Components\System\Components\packages") 
)

[xml]$xml =Get-Content "$PSScriptRoot\Xpand.projects"
$group=$xml.Project.ItemGroup
Write-Host "Starting nuget restore from $currentLocation\Restore-Nuget.ps1...." -f "Blue"

$rootPath="$PSScriptRoot\..\.."


. $PSScriptRoot\Utils.ps1
$projects=($group.DemoSolutions|GetProjects)+
($group.ModuleProjects|GetProjects)+
($group.HelperProjects|GetProjects)+
($group.VSAddons|GetProjects)+
($group.EasyTestProjects|GetProjects)+
($group.CoreProjects|GetProjects)


$psObj=[PSCustomObject]@{
    PackagesDirectory = (Get-Item "$PSScriptRoot\..\_third_party_assemblies\Packages").FullName
    packageSources=[system.string]::join(";",$packageSources)
    projects=$projects
} 
$nuget=Get-XNugetPath
$psObj.Projects|Invoke-XParallel -ActivityName "Restoring Nugets" -VariablesToImport @("psObj","nuget") -Script {
    & $nuget Restore $_ -PackagesDirectory $psObj.PackagesDirectory -source $psObj.packageSources
}

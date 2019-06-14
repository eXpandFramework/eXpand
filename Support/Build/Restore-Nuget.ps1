param(
    [string[]]$packageSources=@("https://api.nuget.org/v3/index.json","https://xpandnugetserver.azurewebsites.net/nuget",(Get-Feed -dx)) 
)

[xml]$xml =Get-Content "$PSScriptRoot\Xpand.projects"
$group=$xml.Project.ItemGroup
Write-Host "Starting nuget restore from $currentLocation\Restore-Nuget.ps1...." -f "Blue"

$rootPath="$PSScriptRoot\..\.."


. $PSScriptRoot\Utils.ps1
# $projects=($group.DemoSolutions|GetProjects)+
$projects=
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
Import-module XpandPwsh -Force -Prefix X
$nuget=Get-XNugetPath
$psObj.Projects|Invoke-XParallel -stepinterval 100 -ActivityName "Restoring Nugets" -VariablesToImport @("psObj","nuget") -Script {
# $psObj.Projects|foreach {
    write-output "Restoring $_"
    & $nuget Restore $_ -PackagesDirectory $psObj.PackagesDirectory -source $psObj.packageSources
}

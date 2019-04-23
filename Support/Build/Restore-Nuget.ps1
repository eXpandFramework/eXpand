param(
    [string[]]$packageSources=@("https://api.nuget.org/v3/index.json","https://xpandnugetserver.azurewebsites.net/nuget","C:\Program Files (x86)\DevExpress 18.2\Components\System\Components\packages") 
)

[xml]$xml =Get-Content "$PSScriptRoot\Xpand.projects"
$group=$xml.Project.ItemGroup
Write-Host "Starting nuget restore from $currentLocation\Restore-Nuget.ps1...." -f "Blue"

$rootPath="$PSScriptRoot\..\.."
Update-XHintPath -OutputPath "$rootPath\Xpand.Dll" -SourcesPath $rootPath -filter "DevExpress*"
get-childitem $rootPath "packages.config" -Recurse|ForEach-Object{
    $xml=Get-Content $_.FullName 
    $xml.packages.Package.Id|Group-Object |Where-Object{$_.Count -gt 1}|ForEach-Object{
        $_.Group|Select-Object -skip 1 | ForEach-Object{
            $id=$_
            $project=$xml.packages.Package|Where-Object{$_.id -eq $id}|select -first 1
            $project.parentNode.RemoveChild($project)
        }
    }
    $xml.Save($_.FullName)
}

. $PSScriptRoot\Utils.ps1
$projects=($group.DemoSolutions|GetProjects)+
($group.DemoTesterProjects|GetProjects)+
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
    "Restoring $_ from $($psObj.packageSource) in $($psObj.PackagesDirectory)"
    & $nuget Restore $_ -PackagesDirectory $psObj.PackagesDirectory -source $psObj.packageSources
}

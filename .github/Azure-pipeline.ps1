param(
   $repository="/eXpand.lab",
   $DXApiFeed=$env:DxFeed,
   $NuGetApiKey,
   $artifactstagingdirectory
)

$WorkingDirectory="$PSScriptRoot\.."
if ($repository -like "*/eXpand.lab"){
   "Finding Version.."
   $version=(& $WorkingDirectory\Support\build\go.ps1)|Select-Object -Last 1
   
   $repository="eXpand.lab"
}
elseif ($repository -like "*/eXpand"){
   $file=Get-Content "$WorkingDirectory\build.ps1" -Raw
   $file -cmatch '-version "(.*)"'
   $version=$Matches[1]
   $repository="eXpand"
}
else{
   throw $repository
}
$version
& "$WorkingDirectory\support\build\go.ps1" -installmodules 
Invoke-Script {
    
   Set-Location $WorkingDirectory
   try {
       dotnet nuget add source "https://api.nuget.org/v3/index.json" --name "nuget.org"
       dotnet nuget add source "$DXApiFeed" --name "DX"
       dotnet nuget add source "https://xpandnugetserver.azurewebsites.net/nuget" --name "Xpand"
   }
   catch { }
   $LASTEXITCODE=0
   dotnet nuget list source
   
}
Set-VsoVariable build.updatebuildnumber $version
Set-Location $WorkingDirectory

# Push-Location $WorkingDirectory\Xpand.Plugins
# Move-PaketSource 0 $DXApiFeed
# Pop-Location


Set-location $WorkingDirectory

[version]$pversion=$version
$pversion=Get-DevExpressVersion $version -Build

"Start build.."
$buildArgs=@{
   packageSources=@("https://api.nuget.org/v3/index.json","https://xpandnugetserver.azurewebsites.net/nuget","$DXApiFeed")
   configuration="Release"
   taskList=@("Release")
   nugetApiKey=$nugetApiKey
   version=$version
   Repository=$repository
}
if ($BetaFeed){
   $buildArgs.Add("Branch","beta")
}
$buildArgs
& "$WorkingDirectory\support\build\go.ps1" @buildArgs 



if ($LastExitCode){
   exit $LastExitCode
}

if ($artifactstagingdirectory){
   Write-HostFormatted "Copy to artifacts" -Section
   Get-ChildItem "$WorkingDirectory\Build\_Package\$Version" -Recurse |ForEach-Object{
      Copy-Item $_.FullName -Destination $artifactstagingdirectory
   }
}

Write-HostFormatted "Restore DX Sources" -Section
$env:DxFeed=$DXApiFeed
$DXVersion=Get-DevExpressVersion (Get-DevExpressVersion)
Set-Location $WorkingDirectory
Move-PaketSource 0 "C:\Program Files (x86)\DevExpress $DXVersion\Components\System\Components\Packages"
# Push-Location $WorkingDirectory\Xpand.Plugins
# Move-PaketSource 0 "C:\Program Files (x86)\DevExpress $DXVersion\Components\System\Components\Packages"
# Pop-Location
Write-HostFormatted "Exit"
exit 
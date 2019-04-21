param(
   $repository,
   $DXApiFeed,
   $NuGetApiKey,
   $artifactstagingdirectory,
   $BetaFeed
)
$VerbosePreference="continue"
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
Write-Verbose -Verbose "##vso[build.updatebuildnumber]$version"
"Start build.."

$buildArgs=@{
   packageSources=@("https://api.nuget.org/v3/index.json","https://xpandnugetserver.azurewebsites.net/nuget","$DXApiFeed",$BetaFeed)
   configuration="Release"
   msbuild="C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\msbuild.exe"
   taskList=@("Release")
   nugetApiKey=$nugetApiKey
   version=$version
   Repository=$repository
}
$buildArgs
& "$WorkingDirectory\support\build\go.ps1" @buildArgs 
if ($LastExitCode){
   exit $LastExitCode
}
Get-ChildItem "$WorkingDirectory\Build\_Package\$Version" -Recurse |ForEach-Object{
   Copy-Item $_.FullName -Destination $artifactstagingdirectory
}

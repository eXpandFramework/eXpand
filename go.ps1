$configuration="Release"
$location=Get-Location
set-location "C:\Program Files (x86)\Microsoft Visual Studio\Installer\"
$msbuild = .\vswhere.exe -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
$msbuild = join-path $msbuild 'MSBuild\15.0\Bin\MSBuild.exe'
Copy-Item $msbuild "$location\MSBuild.exe"
$assemblyInfo="Xpand\Xpand.Utils\Properties\XpandAssemblyInfo.cs"
set-location $location
$version=(Get-Content $assemblyInfo -ErrorAction Stop | Select-String 'public const string Version = \"([^\"]*)')[0].Matches.Groups[1].Value
Invoke-Expression "msbuild Xpand.Build  /fl /t:Release /p:Version=$version;Configuration=$configuration"
Remove-Item msbuild.exe

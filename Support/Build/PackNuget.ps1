
$basePath=[System.IO.Path]::GetFullPath( "$PSScriptRoot\..\..\")
Set-Location $basePath
$nuspecFiles= "$basePath\Support\Nuspec"
$assemblyInfo="$basePath\Xpand\Xpand.Utils\Properties\XpandAssemblyInfo.cs"
$matches = Get-Content $assemblyInfo -ErrorAction Stop | Select-String 'public const string Version = \"([^\"]*)'
$XpandVersion=$matches[0].Matches.Groups[1].Value 
$nupkgPath= "$basePath\Build\Nuget"
New-Item $nupkgPath -ItemType Directory 
if (!(Test-path "$basePath\build\temp\")){
    New-Item "$basePath\build\temp\" -ItemType Directory
}
Get-ChildItem "$basePath\Xpand.DLL" -Include @('*.pdb','*.dll')| Copy-Item -Destination "$basePath\build\temp\" 
$nuspecFiles=Get-ChildItem -Path $nuspecFiles -Filter *.nuspec
$psObj = [PSCustomObject]@{
    OutputDirectory = $nupkgPath
    Nuspecs          = $nuspecFiles|Select-Object -ExpandProperty FullName 
    version=$XpandVersion
}
$nuget="$(Get-XNugetPath)"
# $psObj.Nuspecs|Invoke-XParallel -VariablesToImport @("psObj","nuget") -ActivityName Packing -script{
$psObj.Nuspecs|foreach{
    Write-Output "Packing $_"
    & $Nuget Pack $_ -version ($psObj.Version) -OutputDirectory ($psObj.OutputDirectory) -BasePath "$basePath\build\temp"
}

$packageDir="$basepath\Build\_package\$XpandVersion"
New-Item $packageDir -ItemType Directory -Force|Out-Null
Compress-Archive -DestinationPath "$packageDir\Nupkg-$XpandVersion.zip" -path "$nupkgPath\*"




    
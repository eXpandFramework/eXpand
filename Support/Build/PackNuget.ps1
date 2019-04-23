param(

)
$basePath=[System.IO.Path]::GetFullPath( "$PSScriptRoot\..\..\")
Set-Location $basePath
$nuspecFiles= "$basePath/Support/Nuspec"
$assemblyInfo="$basePath\Xpand\Xpand.Utils\Properties\XpandAssemblyInfo.cs"
$matches = Get-Content $assemblyInfo -ErrorAction Stop | Select-String 'public const string Version = \"([^\"]*)'
$XpandVersion=$matches[0].Matches.Groups[1].Value 
$nupkgPath= "$PSScriptRoot\..\..\Build\Nuget"
New-Item $nupkgPath -ItemType Directory -ErrorAction SilentlyContinue
$nupkgPath=[System.IO.Path]::GetFullPath($nupkgPath)
Remove-Item "$basePath\build\temp" -Force -Recurse -ErrorAction SilentlyContinue 
New-Item "$basePath\build\temp" -ItemType Directory -ErrorAction SilentlyContinue

Get-ChildItem "$basePath/Xpand.DLL" -Include @('*.pdb','*.dll')| Copy-Item -Destination "$basePath\build\temp\$_" 

& "$PSScriptRoot\UpdateNuspecContainers.ps1"

$supportFolder=$(Split-Path $PSScriptRoot)
$XpandFolder=(Get-Item $supportFolder).Parent.FullName
$nuspecFolder="$supportFolder\Nuspec"
Get-ChildItem $nuspecFolder  -Filter "*.nuspec" | foreach{
    $filePath="$nuspecFolder\$_"
    (Get-Content $filePath).replace('src="\Build', "src=`"$XpandFolder\Build") | Set-Content $filePath -Encoding UTF8
}

Remove-Item "$nupkgPath" -Force -Recurse 

$nuspecFiles=Get-ChildItem -Path $nuspecFiles -Filter *.nuspec

$psObj = [PSCustomObject]@{
    OutputDirectory = $nupkgPath
    Nuspecs          = $nuspecFiles|Select-Object -ExpandProperty FullName 
    version=$XpandVersion
}
$nuget="$(Get-XNugetPath)"
$psObj.Nuspecs|Invoke-XParallel -VariablesToImport @("psObj","nuget") -ActivityName Packing -script{
    & $Nuget Pack $_ -version ($psObj.Version) -OutputDirectory ($psObj.OutputDirectory)
}

Get-ChildItem $nuspecFolder  -Filter "*.nuspec" | ForEach-Object{
    $filePath="$nuspecFolder\$_"
    (Get-Content $filePath).replace("src=`"$XpandFolder\Build",'src="\Build') | Set-Content $filePath -Encoding UTF8
}

$packageDir="$basepath\Build\_package\$XpandVersion"
New-Item $packageDir -ItemType Directory -Force|Out-Null
Compress-Archive -DestinationPath "$packageDir\Nupkg-$XpandVersion.zip" -path "$nupkgPath\*"




    
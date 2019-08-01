Param (
    [string]$XpandFolder=(Get-Item "$PSScriptRoot\..\..").FullName,
    [string]$msbuild="C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\msbuild.exe",
    [string]$DXVersion="0.0.0.0",
    [string]$Source="$(Get-PackageFeed -Nuget);$(Get-Feed -DX)"
)
$ErrorActionPreference = "Stop"

Import-Module XpandPwsh -Force -Prefix X

if ($DXVersion -eq "0.0.0.0"){
    $DXVersion=Get-AssemblyInfoVersion "$PSScriptRoot\..\..\Xpand\Xpand.Utils\Properties\XpandAssemblyInfo.cs"
}

#update version in templates
$version=New-Object System.Version ($DXVersion)
$projectTemplates="$XpandFolder\Xpand.Plugins\Xpand.VSIX\ProjectTemplates"
$tempPath="$projectTemplates\temp"

Get-ChildItem "$projectTemplates\*.zip" -Recurse |ForEach-Object{
    New-Item $tempPath -ItemType Directory -Force|out-null
    Expand-Archive $_.FullName -DestinationPath $tempPath -Force
    Remove-Item $_.FullName  -Force
    $vsTemplate=(Get-ChildItem $tempPath -Filter *.vstemplate | Select-Object -First 1).FullName
    $content=Get-Content $vsTemplate
    $content = $content -ireplace 'eXpandFramework v([^ ]*)', "eXpandFramework v$($version.Major).$($version.Minor)"
    $content = $content -ireplace 'Xpand.VSIX, Version=([^,]*)', "Xpand.VSIX, Version=$($version.ToString())"
    Set-Content $vsTemplate $content
    Compress-7Zip -Path $tempPath -ArchiveFileName $_.FullName 
    Remove-Item $tempPath -Recurse -Force 
}

Get-ChildItem "$XpandFolder\Xpand.Plugins\Xpand.VSIX\ProjectTemplates\*.vstemplate" -Recurse|ForEach-Object{
    $content=Get-Content $_.FullName
    $content = $content -ireplace "TemplateWizard.v([^,]*),", "TemplateWizard.v$($version.Major).$($version.Minor),"
    Set-Content $_.FullName $content
}

#build VSIX
$fileName="$XpandFolder\Xpand.Plugins\Xpand.VSIX\Xpand.VSIX.csproj"
& "$(Get-XNugetPath)" Restore $fileName -PackagesDirectory "$XpandFolder\Support\_third_party_assemblies\Packages" -source $Source 
& "$msbuild" "$fileName" "/p:Configuration=Release;DeployExtension=false;OutputPath=$XpandFolder\Xpand.Dll\Plugins" /v:m 





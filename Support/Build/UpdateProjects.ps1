$ErrorActionPreference = "Stop"
$rootLocation = "$PSScriptRoot\..\..\"
Write-Host "Update Projects"
Set-Location $rootLocation

# Update-HintPath $rootLocation "$rootLocation\Xpand.Dll\" @("Xpand.*","DevExpress.*") @("*.DXCore.*","Xpand.XAF*","Xpand.VersionConverter*")
Get-ChildItem "$rootLocation\Xpand" *.csproj -Recurse | ForEach-Object {
    $fileName = $_.FullName
    [xml]$projXml = Get-Content $fileName
    Update-ProjectSign $projXml $fileName "$rootLocation\Xpand\Xpand.key\xpand.snk"
    Update-ProjectDebugSymbols $projXml
    $projXml.Save($fileName)
} 
Get-ChildItem *.csproj -Recurse | ForEach-Object {
    $fileName = $_.FullName
    [xml]$projXml = Get-Content $fileName
    Remove-ProjectLicenseFile $projXml
    Update-ProjectAutoGenerateBindingRedirects $projXml
    Update-ProjectLanguageVersion $projXml
    $projXml.Save($fileName)
} 




Param (
    [string]$XpandFolder=$(get-item "$PSScriptRoot\..\..").FullName,
    [string]$Version="0.0.0.1"
)

$assemblyInfo="$XpandFolder\Xpand\Xpand.Utils\Properties\XpandAssemblyInfo.cs"
$xpandVersion=Get-XXpandVersion $XpandFolder
Write-Host "xpcandVersion=$xpandVersion ,$Version"
(Get-Content $assemblyInfo).replace($xpandVersion, $Version) | Set-Content $assemblyInfo



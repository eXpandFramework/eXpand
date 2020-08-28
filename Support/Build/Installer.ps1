Param (
    [string]$XpandFolder=(Get-Item "$PSScriptRoot\..\..").FullName,
    [string]$DXVersion="0.0.0.0"
)
function CloneItem{
    [cmdletbinding()]
    param(
        [parameter(ValueFromPipeline=$True,mandatory=$True)]
        [string]$Path,
        [parameter(mandatory=$True)]
        [string] $TargetDir,
        [parameter(mandatory=$True)]
        [string]$SourceDir
    )
    $targetFile = $TargetDir + $Path.SubString($SourceDir.Length);
    
    if (!((Get-Item $Path) -is [System.IO.DirectoryInfo])){
        $dirName=Split-Path $targetFile -Parent
        New-Item -ItemType Directory $dirName -ErrorAction SilentlyContinue
        Copy-Item $Path -destination $targetFile -Force
        Write-Output $targetFile
    }
}


Push-Location "$XpandFolder"
if ($DXVersion -eq "0.0.0.0"){
    $DXVersion=Get-XXpandVersion "$XpandFolder"
}
$installerFolder="$XpandFolder\Build\Installer"
if (test-path $installerFolder){
    Remove-Item $installerFolder -Force -Recurse
}
New-Item -ItemType Directory $installerFolder -Force
$packageFolder ="$XpandFolder\Build\_Package\$DXVersion\"
New-Item -ItemType Directory $packageFolder -Force


Copy-Item "$XpandFolder\Xpand.DLL\Plugins\Xpand.VSIX.vsix" "$packageFolder\Xpand.VSIX-$DXVersion.vsix"
Copy-Item "$XpandFolder\Xpand.DLL\Plugins\Xpand.VSIX.vsix" "$installerfolder\Xpand.VSIX-$DXVersion.vsix"



Pop-Location



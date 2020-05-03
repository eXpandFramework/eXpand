param(
    $TargetPath="C:\Work\eXpandFramework\Issues\Solution10\Solution10.Module\bin\Debug\Solution10.Module.dll"
)
$pdbname = "$((Get-ChildItem $PSScriptRoot *.targets).BaseName).pdb"
$targetDir=[System.IO.Path]::GetDirectoryName($TargetPath)
$pdbPath=Get-ChildItem $PSScriptRoot\..\lib *.pdb -Recurse
if (!(Test-Path "$targetDir\$pdbname")){
    $pdbPath|Copy-Item -Destination $targetDir
}
else{
    if (!(Compare-Object $pdbPath "$targetDir\$pdbname" -Property Length, LastWriteTime)){
        Copy-Item $pdbPath "$targetDir\$pdbname" -Force
    }
}

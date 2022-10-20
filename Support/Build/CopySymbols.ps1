param(
    $TargetPath="C:\Work\eXpandFramework\Issues\Solution10\Solution10.Module\bin\Debug\Solution10.Module.dll",
    $ProjectPath="DoNotRemove"
)
$msSpeechPath="$PSScriptRoot\..\..\..\microsoft.cognitiveservices.speech"
if (Test-Path $msSpeechPath){
    Get-ChildItem $msSpeechPath "Microsoft.CognitiveServices.Speech.csharp.dll" -Recurse|Where-Object{
        $_.Directory.Name -eq "netstandard2.0" 
    }|ForEach-Object{
        $corelib="$($_.DirectoryName)\Microsoft.CognitiveServices.Speech.core.dll"
        if (!(Test-Path $corelib)){
            try {
                Copy-Item $_.FullName $corelib -ErrorAction SilentlyContinue
                $LASTEXITCODE=0
            }
            catch {
                
            }
        }
    }
}

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

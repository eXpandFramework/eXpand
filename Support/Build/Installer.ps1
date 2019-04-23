Param (
    [string]$XpandFolder=(Get-Item "$PSScriptRoot\..\..").FullName,
    [string]$DXVersion="0.0.0.0"
)
. "$PSScriptRoot\Utils.ps1"
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

#Create Xpand.DLL
New-Item -ItemType Directory -Path "$installerFolder\Xpand.DLL" -Force
Copy-Item -Path ".\Xpand.DLL\Xpand.ExpressApp.ModelEditor.exe" -Destination "$installerFolder\Xpand.DLL\Xpand.ExpressApp.ModelEditor.exe"
Get-ChildItem -Path ".\Xpand.DLL" -Include "*.*" | Where-Object{
    $fullName=$_.FullName
    (("*.dll","*.exe","*.config","*.pdb"|Where-Object{$fullName -like $_}).Length -gt 0) -and ($fullName -notlike "*\Plugins\*")
} | 
Copy-Item -Destination "$installerFolder\Xpand.DLL\" -Force
Compress-Archive -DestinationPath $packageFolder\Xpand-lib-$DXVersion.zip -path "$installerFolder\Xpand.DLL\*" 
"Creating Xpand-lib-$DXVersion.zip"

Copy-Item "$XpandFolder\Xpand.DLL\Plugins\Xpand.VSIX.vsix" "$packageFolder\Xpand.VSIX-$DXVersion.vsix"
Copy-Item "$XpandFolder\Xpand.DLL\Plugins\Xpand.VSIX.vsix" "$installerfolder\Xpand.VSIX-$DXVersion.vsix"

$sourceFolder="$installerFolder\Source\"
Get-ChildItem $XpandFolder -recurse -Include "*.*" |Where-Object{
    $fullName=$_.FullName
    (("*\Build\Installer*","*\Build\_Package*", "*\.git\*",'*\$RECYCLE.BIN\*',"*\System Volume Information\*","*\packages\*",
    "*\dxbuildgenerator\packages\*","*\_Resharper\*","*\ScreenCapture\*","*.log","*web_view.html","win_view.html",
    "web_view.jpeg","win_view.jpeg","*\Xpand.DLL\*","*.user","*\.vs\*","*.suo","*\bin\*","*\obj\*","*.docstates","*teamcity*","*.gitattributes","*.gitmodules","*.gitignore"|
    Where-Object{$fullName -like $_}).Length -eq 0)
} | ForEach-Object {CloneItem $_ -TargetDir $sourceFolder -SourceDir $XpandFolder  }
Remove-Item "$sourceFolder\build" -Recurse -Force 
Compress-Archive -Path "$sourceFolder\*"  -DestinationPath "$installerFolder\Source.zip"
"Creating source.zip"
Remove-Item $sourceFolder -Force -Recurse

Copy-Item "$installerFolder\Source.zip" -Destination "$packageFolder\Xpand-Source-$DXVersion.zip"

Pop-Location



call clear.bat
echo Build Target
%msbuild% /nologo /p:Configuration=%configuration% /fl Xpand.build 

echo Installing assemblies to GAC...
xcopy "_third_party_assemblies\GACInstaller.exe" ".\Xpand.DLL\*.*" /S /Y /H /I
call ".\Xpand.DLL\GACInstaller.exe" 

echo Installing Toolbox Items...
call buildproject.cmd Xpand.ToolboxCreator ".\ToolBoxCreator\Xpand.ToolboxCreator.csproj"
call ".\Xpand.DLL\Xpand.ToolBoxCreator.exe"



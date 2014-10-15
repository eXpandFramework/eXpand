call clear.bat
echo Build Target
%msbuild% /nologo /p:Configuration=%configuration% /fl /p:BatchCall=true /v:m .\Xpand.build 

echo Installing assemblies to GAC...
xcopy ".\Support\_third_party_assemblies\GACInstaller.exe" ".\Xpand.DLL\*.*" /S /Y /H /I
call ".\Xpand.DLL\GACInstaller.exe" Xpand 

echo Installing Toolbox Items...
call buildproject.cmd Xpand.ToolboxCreator ".\Support\ToolBoxCreator\Xpand.ToolboxCreator.csproj"
call ".\Xpand.DLL\Xpand.ToolBoxCreator.exe"



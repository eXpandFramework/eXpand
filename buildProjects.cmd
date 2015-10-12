call clear.bat
echo Build Target
%msbuild% /nologo /p:Configuration=%configuration% /fl /p:BatchCall=true /v:m .\Xpand.build

echo Installing assemblies to GAC...
call ".\Xpand.DLL\GACInstaller.exe" -r Xpand.*

echo Installing Toolbox Items...
call buildproject.cmd Xpand.ToolboxCreator ".\Support\ToolBoxCreator\Xpand.ToolboxCreator.csproj"
call ".\Xpand.DLL\Xpand.ToolBoxCreator.exe"

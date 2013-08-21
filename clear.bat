
@echo off

call defines.bat
%msbuild% /nologo /t:Rebuild /verbosity:quiet /p:Configuration=%configuration% ".\ToolBoxCreator\Xpand.ToolboxCreator.csproj"
call ".\Xpand.DLL\Xpand.ToolBoxCreator.exe" u
call GACUnistall.bat

pause
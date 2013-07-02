
@echo off

call defines.bat
call .\Xpand.DLL\Xpand.ToolBoxCreator.exe u
copy .\_third_party_assemblies\GACInstaller.exe .\Xpand.DLL\*.*
call .\Xpand.DLL\GACInstaller.exe "" u

pause